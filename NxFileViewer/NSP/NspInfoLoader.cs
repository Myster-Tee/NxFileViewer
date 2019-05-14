using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Emignatik.NxFileViewer.Hactool;
using Emignatik.NxFileViewer.Logging;
using Emignatik.NxFileViewer.NSP.Models;
using Emignatik.NxFileViewer.NxFormats.CNMT;
using Emignatik.NxFileViewer.NxFormats.CNMT.Models;
using Emignatik.NxFileViewer.NxFormats.CNMT.Structs;
using Emignatik.NxFileViewer.NxFormats.NACP;
using Emignatik.NxFileViewer.NxFormats.NACP.Structs;
using Emignatik.NxFileViewer.NxFormats.NCA;
using Emignatik.NxFileViewer.NxFormats.NCA.Models;
using Emignatik.NxFileViewer.NxFormats.NCA.Structs;
using Emignatik.NxFileViewer.NxFormats.PFS0;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Utils;

namespace Emignatik.NxFileViewer.NSP
{
    public class NspInfoLoader
    {
        private readonly HactoolHelper _hactoolHelper;
        private readonly TempDirMgr _tempDirMgr;
        private const string CNMT_NCA = ".cnmt.nca";

        public NspInfoLoader(HactoolHelper hactoolHelper, TempDirMgr tempDirMgr)
        {
            _hactoolHelper = hactoolHelper ?? throw new ArgumentNullException(nameof(hactoolHelper));
            _tempDirMgr = tempDirMgr ?? throw new ArgumentNullException(nameof(tempDirMgr));
        }

        public NspInfo Load(string nspFilePath)
        {
            if (nspFilePath == null)
                throw new ArgumentNullException(nameof(nspFilePath));

            if (!KeySetProviderService.TryGetKeySet(out var keySet, out var errorMessage))
                throw new Exception($"Can't load NSP data: {errorMessage}");

            _tempDirMgr.Initialize();

            // Read the files contained in NSP file (PFS0)
            using (var nspReader = Pfs0Reader.FromFile(nspFilePath))
            {
                var fileDefinitions = nspReader.ReadFileDefinitions();

                Pfs0NcaFile ncaMetaFile = null;
                var files = new List<Pfs0File>();
                foreach (var fileDefinition in fileDefinitions)
                {
                    Pfs0File pfs0File;
                    var fileName = fileDefinition.FileName ?? "";
                    if (fileName.EndsWith(".nca", StringComparison.InvariantCultureIgnoreCase))
                    {
                        using (var ncaReader = new NcaReader(new NcaDecryptionStream(nspReader.GetFileStream(fileDefinition), keySet)))
                        {
                            var ncaHeader = ncaReader.ReadHeader();

                            var pfs0NcaFile = new Pfs0NcaFile
                            {
                                Definition = fileDefinition,
                                Header = ncaHeader,
                            };
                            pfs0File = pfs0NcaFile;

                            // Find the "*.cnmt.nca" file definition which is the container of the *.cnmt meta file
                            if (fileName.EndsWith(CNMT_NCA, StringComparison.InvariantCultureIgnoreCase))
                                ncaMetaFile = pfs0NcaFile;
                        }
                    }
                    else
                    {
                        pfs0File = new Pfs0File
                        {
                            Definition = fileDefinition,
                        };

                    }

                    files.Add(pfs0File);
                }

                // We assume the CNMT file as mandatory, hope it is true (some NSP doesn't contain the *.cnmt.xml file)
                if (ncaMetaFile == null)
                    throw new Exception($"No \"*{CNMT_NCA}\" found in NSP.");

                // We assume the CNMT file is always of Meta type 
                if (ncaMetaFile.Header.ContentType != NcaContentType.META)
                    throw new Exception($"File \"{ncaMetaFile.Definition.FileName}\" is not of the expected content type \"{NcaContentType.META}\".");

                // We assume the CNMT file always contains a section 0
                if (ncaMetaFile.Header.GetSectionHeaderByIndex(NcaSectionIndex.SECTION_0) == null)
                    throw new Exception($"Meta file \"{ncaMetaFile.Definition.FileName}\" doesn't contain the expected section \"{NcaSectionIndex.SECTION_0}\".");

                // Extracts the encrypted "*.cnmt.nca" file
                nspReader.SaveFileToDir(ncaMetaFile.Definition, _tempDirMgr.TempDirPath, out var cnmtNcaFilePath);

                // Decrypt and extract "section 0" of "*.cnmt.nca" file to dir (should create exactly one *.cnmt files in the target directory)
                var metaTargetDir = _tempDirMgr.CreateSubDir("Meta_Section_0");
                _hactoolHelper.NcaDecryptSectionToDir(cnmtNcaFilePath, NcaSection.SECTION_0, metaTargetDir);

                // Search for the decrypted cnmt meta file
                var cnmtMetaFilePath = FindSingleCnmtFile(metaTargetDir);

                // Load meta file header and get retrieve the id (file name without ext) of the NCA file of "Control" type (NCA containing logos and localized app names)
                var cnmtHeader = ReadCnmtAndFindNcaControlId(cnmtMetaFilePath, out var ncaControlId);

                NcaControlContent ncaControlContent = null;
                if (ncaControlId != null) // ncaControlId can be null in add-ons
                {
                    // Search the NCA file in the PFS0
                    var ncaControlFile = FindNcaById(ncaControlId, files);
                    if (ncaControlFile == null)
                        throw new Exception($"NCA Meta file \"{ncaMetaFile.Definition.FileName}\" is referencing the NCA file \"{ncaControlId}\" of type \"{CnmtContentType.Control}\" which couldn't be found in the NSP.");

                    // Load title information and icons
                    ncaControlContent = LoadNcaControlContent(ncaControlFile, nspReader);
                }

                var nspInfo = new NspInfo
                {
                    Files = files.ToArray(),
                    CnmtHeader = cnmtHeader,
                    NcaMetaHeader = ncaMetaFile.Header,
                    NcaControlContent = ncaControlContent
                };

                return nspInfo;
            }
        }

        /// <summary>
        /// Load interesting information contained in the NCA file of type <see cref="NcaContentType.CONTROL"/>
        /// </summary>
        /// <param name="ncaControlFile"></param>
        /// <param name="nspReader"></param>
        /// <returns></returns>
        private NcaControlContent LoadNcaControlContent(Pfs0NcaFile ncaControlFile, Pfs0Reader nspReader)
        {
            if (ncaControlFile.Header.ContentType != NcaContentType.CONTROL)
                throw new Exception($"File \"{ncaControlFile.Definition.FileName}\" is not of the expected content type \"{NcaContentType.CONTROL}\".");

            if (ncaControlFile.Header.GetSectionHeaderByIndex(NcaSectionIndex.SECTION_0) == null)
                throw new Exception($"Control file \"{ncaControlFile.Definition.FileName}\" doesn't contain the expected section \"{NcaSectionIndex.SECTION_0}\".");

            // Extract the encrypted NCA control file
            nspReader.SaveFileToDir(ncaControlFile.Definition, _tempDirMgr.TempDirPath, out var ncaControlFilePath);

            // Decrypt and extract the content of the NCA control file
            var controlTargetDir = _tempDirMgr.CreateSubDir("Control_Section_0");
            _hactoolHelper.NcaDecryptSectionToDir(ncaControlFilePath, NcaSection.SECTION_0, controlTargetDir);

            // Check control.nacp file exists
            var nacpControlFilePath = Path.Combine(controlTargetDir, "control.nacp");
            if (!File.Exists(nacpControlFilePath))
            {
                Logger.LogError($"Expected NACP file \"{nacpControlFilePath}\" not found.");
                return null;
            }

            using (var nacpReader = NacpReader.FromFile(nacpControlFilePath))
            {
                var ncaControlContent = new NcaControlContent
                {
                    NacpContent = nacpReader.ReadContent(),
                    Icons = ScanIcons(controlTargetDir),
                };

                return ncaControlContent;
            }
        }

        private static List<LocalizedIcon> ScanIcons(string controlTargetDir)
        {
            const string ICON_FILE_PREFIX = "icon_";

            var icons = new List<LocalizedIcon>();
            foreach (var datFileInfo in new DirectoryInfo(controlTargetDir).GetFiles("*.dat"))
            {
                var fileName = datFileInfo.Name;
                if (!fileName.StartsWith(ICON_FILE_PREFIX, true, CultureInfo.InvariantCulture))
                {
                    Logger.LogWarning($"Found a *.dat file \"{fileName}\" which doesn't seem to be an icon.");
                    continue;
                }

                var langName = Path.GetFileNameWithoutExtension(fileName.Substring(ICON_FILE_PREFIX.Length));
                if (!Enum.TryParse(langName, true, out NacpLanguage language))
                {
                    Logger.LogWarning($"Found a *.dat file \"{fileName}\" which doesn't match any of the languages.");
                    continue;
                }

                icons.Add(new LocalizedIcon
                {
                    IconFilePath = datFileInfo.FullName,
                    Language = language,
                });
            }

            return icons;
        }

        private static Pfs0NcaFile FindNcaById(string ncaControlId, IEnumerable<Pfs0File> files)
        {
            var searchedNcaFile = ncaControlId + ".nca";
            return files.FirstOrDefault(file => file.Definition.FileName == searchedNcaFile) as Pfs0NcaFile;
        }

        private static CnmtHeader ReadCnmtAndFindNcaControlId(string cnmtMetaFilePath, out string ncaControlId)
        {
            using (var cnmtReader = CnmtReader.FromFile(cnmtMetaFilePath))
            {
                var cnmtHeader = cnmtReader.Read();

                var controlRecords = cnmtHeader.ContentRecords.Where(cr => cr.Type == CnmtContentType.Control).ToArray();
                if (controlRecords.Length > 1)
                    throw new Exception($"More than one record of type \"{CnmtContentType.Control}\" found in meta file \"{cnmtMetaFilePath}\".");

                ncaControlId = controlRecords.Length == 1 ? controlRecords[0].NcaId : null;

                return cnmtHeader;
            }
        }

        private static string FindSingleCnmtFile(string dirPath)
        {
            var files = Directory.GetFiles(dirPath, "*.cnmt");
            if (files.Length < 1)
                throw new FileNotFoundException($"No cnmt file found in directory \"{dirPath}\".");

            if (files.Length > 1)
                throw new FileNotFoundException($"More than one cnmt file found in directory \"{dirPath}\".");
            return files[0];
        }


    }
}