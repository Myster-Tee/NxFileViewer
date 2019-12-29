using System;
using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.NSP.Models;
using Emignatik.NxFileViewer.Utils;
using LibHac;
using LibHac.Fs;
using LibHac.FsSystem;
using LibHac.FsSystem.NcaUtils;
using LibHac.Ncm;
using log4net;


namespace Emignatik.NxFileViewer.NSP
{
    public class NspInfoLoader
    {
        private readonly Keyset _keyset;
        private readonly ILog _log;

        public NspInfoLoader(Keyset keyset, ILog log = null)
        {
            _keyset = keyset ?? throw new ArgumentNullException(nameof(keyset));
            _log = log;
        }

        public NspInfo Load(string nspFilePath)
        {
            if (nspFilePath == null)
                throw new ArgumentNullException(nameof(nspFilePath));

            var nspInfo = new NspInfo();

            using (var localFile = new LocalFile(nspFilePath, OpenMode.Read))
            {
                var fileStorage = new FileStorage(localFile);

                var nspPartition = new PartitionFileSystem(fileStorage);

                nspInfo.Files = LoadFiles(nspPartition, _keyset, out var openedNcas).ToArray();

                nspInfo.Cnmts = LoadCnmts(openedNcas).ToArray();

                return nspInfo;
            }
        }

        private ControlPartitionInfo LoadControlInfo(IEnumerable<OpenedNca> openedNcas, string linkedNcaControlId)
        {

            if (string.IsNullOrEmpty(linkedNcaControlId))
                return null;

            var expectedNcaFileName = linkedNcaControlId + ".nca";

            var matchingOpenedNcas = openedNcas.Where(openedNca => string.Equals(expectedNcaFileName, openedNca.FileName, StringComparison.InvariantCultureIgnoreCase)).ToArray();
            if (matchingOpenedNcas.Length < 1)
            {
                _log?.Error($"Referenced NCA \"{expectedNcaFileName}\" of content type \"{NcaContentType.Control}\" is missing.");
                return null;
            }

            if (matchingOpenedNcas.Length > 1)
                _log?.Error($"NCA \"{expectedNcaFileName}\" of content type \"{NcaContentType.Control}\" was found more than once (found \"{matchingOpenedNcas.Length}\" times), only the first one will be considered.");

            var controlOpenedNca = matchingOpenedNcas[0];
            var controlNca = controlOpenedNca.Nca;
            var controlNcaHeader = controlNca.Header;
            if (controlNcaHeader.ContentType != NcaContentType.Control)
            {
                _log?.Error($"NCA \"{controlOpenedNca.FileName}\" is not of the expected content type \"{NcaContentType.Control}\".");
                return null;
            }

            var controlFs = controlNca.OpenFileSystem(controlNcaHeader.ContentIndex, IntegrityCheckLevel.None);
            return ExtractControlInfo(controlFs);
        }

        private List<CnmtInfo> LoadCnmts(IReadOnlyCollection<OpenedNca> openedNcas)
        {
            //TODO: log warnings for meta nca not ending with cnmt.nca?

            var cnmts = new List<CnmtInfo>();

            foreach (var openedNca in openedNcas)
            {
                if (!openedNca.FileName.EndsWith(".cnmt.nca", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                if (openedNca.Nca.Header.ContentType != NcaContentType.Meta)
                {
                    _log?.Error($"Invalid NCA file \"{openedNca.FileName}\", content is not of the expected type \"{NcaContentType.Meta}\" (found \"{openedNca.Nca.Header.ContentType}\").");
                    continue;
                }

                var cnmtFs = openedNca.Nca.OpenFileSystem(openedNca.Nca.Header.ContentIndex, IntegrityCheckLevel.None);
                var cnmtFileEntries = cnmtFs.EnumerateEntries().Where(entry => (entry.Name ?? "").EndsWith(".cnmt", StringComparison.InvariantCultureIgnoreCase)).ToArray();


                if (cnmtFileEntries.Length < 1)
                {
                    _log?.Error($"No CNMT file found in NCA \"{openedNca.FileName}\".");
                    continue;
                }

                if (cnmtFileEntries.Length > 1)
                {
                    _log?.Error($"More than one CNMT file found in NCA \"{openedNca.FileName}\".");
                    continue;
                }

                var cnmtFileEntry = cnmtFileEntries[0];

                using (var cnmtFile = cnmtFs.OpenFile(cnmtFileEntry.FullPath, OpenMode.Read))
                {
                    using (var cnmtStream = cnmtFile.AsStream())
                    {
                        var cnmt = new Cnmt(cnmtStream);

                        var linkedNcaControlIds = cnmt.ContentEntries.Where(entry => entry.Type == ContentType.Control).Select(entry => ByteArrayExt.ToHexString(entry.NcaId)).ToArray();

                        string linkedNcaControlId;
                        if (linkedNcaControlIds.Length > 1)
                        {
                            _log?.Error($"CNMT file \"{cnmtFileEntry.FullPath}\" contained in NCA \"{openedNca.FileName}\" was not expected to reference more than one NCA of content type \"{ContentType.Control}\". Only the first reference will be considered.");
                            linkedNcaControlId = linkedNcaControlIds[0];
                        }
                        else
                        {
                            linkedNcaControlId = linkedNcaControlIds.Length == 1 ? linkedNcaControlIds[0] : null;
                        }

                        cnmts.Add(new CnmtInfo
                        {
                            FilePath = cnmtFileEntry.FullPath,
                            Type = cnmt.Type,
                            TitleId = cnmt.TitleId.ToHex(),
                            TitleVersion = cnmt.TitleVersion.Version,
                            LinkedNcaControlId = linkedNcaControlId,
                            ControlPartition = LoadControlInfo(openedNcas, linkedNcaControlId)
                        });
                    }
                }
            }

            return cnmts;
        }

        //TODO: maybe expose Nintendo logo?
        //if (nca.Header.ContentType == ContentType.Program)
        //{
        //    if (nca.CanOpenSection(NcaSectionType.Logo))
        //    {
        //        var openFileSystem = nca.OpenFileSystem(NcaSectionType.Logo, IntegrityCheckLevel.None) as PartitionFileSystem;

        //        foreach (var entry in openFileSystem.Files)
        //        {
        //            var asStream = openFileSystem.OpenFile(entry, OpenMode.Read).AsStream();

        //            var fileStream = File.Create(@"Temp/" + entry.Name);

        //            asStream.CopyTo(fileStream);
        //            fileStream.Dispose();
        //        }
        //        //foreach (var f in openFileSystem)
        //        //{

        //        //}
        //    }
        //}

        private class OpenedNca
        {
            public string FileName { get; set; }

            public Nca Nca { get; set; }
        }

        private static List<PfsFile> LoadFiles(PartitionFileSystem partition, Keyset keyset, out List<OpenedNca> openedNcas)
        {
            openedNcas = new List<OpenedNca>();

            var files = new List<PfsFile>();
            foreach (var nspFileEntry in partition.Files)
            {
                var fileName = nspFileEntry.Name ?? "";

                PfsFile pfsFile;
                if (fileName.EndsWith(".nca", StringComparison.InvariantCultureIgnoreCase))
                {
                    var openFile = partition.OpenFile(nspFileEntry, OpenMode.Read);

                    var nca = new Nca(keyset, new FileStorage(openFile));
                    openedNcas.Add(new OpenedNca
                    {
                        Nca = nca,
                        FileName = fileName,
                    });

                    var ncaHeader = nca.Header;
                    var definedSections = GetDefinedSectionsInfo(ncaHeader);

                    var pfsNcaFile = new PfsNcaFile
                    {
                        Name = nspFileEntry.Name,
                        ContentType = ncaHeader.ContentType,
                        TitleId = ncaHeader.TitleId,
                        SdkVersion = ncaHeader.SdkVersion.ToString(),
                        DefinedSections = definedSections.ToArray(),
                        DistributionType = ncaHeader.DistributionType,
                    };
                    pfsFile = pfsNcaFile;
                }
                else
                {
                    pfsFile = new PfsFile();
                }

                pfsFile.Name = nspFileEntry.Name;
                pfsFile.Size = nspFileEntry.Size;

                files.Add(pfsFile);
            }

            return files;
        }

        private static List<NcaSectionInfo> GetDefinedSectionsInfo(NcaHeader ncaHeader)
        {
            var definedSections = new List<NcaSectionInfo>();

            for (var i = 0; i < 4; i++)
            {
                if (!ncaHeader.IsSectionEnabled(i))
                    continue;

                var fsHeader = ncaHeader.GetFsHeader(i);

                definedSections.Add(new NcaSectionInfo
                {
                    Index = i,
                    FormatType = fsHeader.FormatType,
                    EncryptionType = fsHeader.EncryptionType,
                });
            }

            return definedSections;
        }

        /// <summary>
        /// From the given RomFS section of the NCA of content type "ControlPartition", retrieves:
        /// -> Info from "control.nacp" (like titles, display version, etc.)
        /// -> Localized title icons
        /// </summary>
        /// <param name="controlFs"></param>
        private ControlPartitionInfo ExtractControlInfo(IFileSystem controlFs)
        {
            NacpInfo nacpInfo = null;
            var icons = new List<IconInfo>();
            const string ICON_FILE_PREFIX = "icon_";
            const string ICON_FILE_EXT = ".dat";

            foreach (var controlFileEntry in controlFs.EnumerateEntries())
            {
                var fileName = controlFileEntry.Name ?? "";
                if (string.Equals(fileName, "control.nacp", StringComparison.InvariantCultureIgnoreCase))
                {
                    using (var nacpFile = controlFs.OpenFile(controlFileEntry.FullPath, OpenMode.Read))
                    {
                        using (var nacpStream = nacpFile.AsStream())
                        {
                            var nacp = new Nacp(nacpStream);

                            var titleInfos = new List<TitleInfo>();

                            foreach (var description in nacp.Descriptions)
                            {
                                if (!string.IsNullOrEmpty(description.Title))
                                {
                                    titleInfos.Add(new TitleInfo
                                    {
                                        AppName = description.Title,
                                        Publisher = description.Developer,
                                        Language = (NacpLanguage)description.Language,
                                    });
                                }
                            }

                            nacpInfo = new NacpInfo
                            {
                                DisplayVersion = nacp.DisplayVersion,
                                Titles = titleInfos.ToArray(),
                            };
                        }
                    }
                }
                else if (fileName.StartsWith(ICON_FILE_PREFIX, StringComparison.InvariantCultureIgnoreCase) && fileName.EndsWith(ICON_FILE_EXT, StringComparison.InvariantCultureIgnoreCase))
                {
                    var langName = fileName.Substring(ICON_FILE_PREFIX.Length, fileName.Length - ICON_FILE_PREFIX.Length - ICON_FILE_EXT.Length);
                    if (!Enum.TryParse(langName, true, out NacpLanguage language))
                    {
                        _log?.Warn($"Found a *.dat file \"{fileName}\" which doesn't match any of the languages.");
                        continue;
                    }

                    byte[] bytes;
                    using (var iconFile = controlFs.OpenFile(controlFileEntry.FullPath, OpenMode.Read))
                    {
                        bytes = new byte[iconFile.GetSize()];
                        iconFile.AsStream().Read(bytes, 0, bytes.Length);
                    }

                    var iconInfo = new IconInfo
                    {
                        Image = bytes,
                        Language = language,
                    };

                    icons.Add(iconInfo);
                }
            }

            return new ControlPartitionInfo
            {
                Nacp = nacpInfo,
                Icons = icons.ToArray()
            };

        }

    }
}