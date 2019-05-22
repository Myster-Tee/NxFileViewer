using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using Emignatik.NxFileViewer.Logging;
using Emignatik.NxFileViewer.NSP.Models;
using Emignatik.NxFileViewer.Utils;
using LibHac;
using LibHac.Fs;
using LibHac.Fs.NcaUtils;
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

                nspInfo.Files = GetPfsFilesInfo(nspPartition, _keyset, out var openedNcas).ToArray();

                nspInfo.Cnmts = LoadCnmtsInfo(openedNcas).ToArray();

                nspInfo.Controls = LoadControlsInfo(nspInfo.Cnmts, openedNcas).ToArray();


                var files = new List<PfsFile>();
                foreach (var nspFileEntry in nspPartition.Files)
                {
                    var fileName = nspFileEntry.Name ?? "";

                    PfsFile pfsFile;
                    if (fileName.EndsWith(".nca", StringComparison.InvariantCultureIgnoreCase))
                    {
                        using (var openFile = nspPartition.OpenFile(nspFileEntry, OpenMode.Read))
                        {
                            var nca = new Nca(_keyset, new FileStorage(openFile));
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

                            if (fileName.EndsWith(".cnmt.nca", StringComparison.InvariantCultureIgnoreCase))
                            {
                                if (ncaHeader.ContentType != ContentType.Meta)
                                    throw new Exception($"\"{fileName}\" is not of the expected type \"{ContentType.Meta}\"");

                                var cnmtFs = nca.OpenFileSystem(ncaHeader.ContentIndex, IntegrityCheckLevel.None);

                                var cnmtFileEntries = cnmtFs.OpenDirectory("/", OpenDirectoryMode.Files).Read().Where(entry => (entry.Name ?? "").EndsWith(".cnmt", StringComparison.InvariantCultureIgnoreCase)).ToArray();

                                if (cnmtFileEntries.Length < 1)
                                    throw new Exception($"No CNMT file found in NCA \"{fileName}\".");
                                if (cnmtFileEntries.Length > 1)
                                    throw new Exception($"More than one CNMT file found in NCA \"{fileName}\".");

                                var cnmtFileEntry = cnmtFileEntries[0];

                                using (var cnmtFile = cnmtFs.OpenFile(cnmtFileEntry.FullPath, OpenMode.Read))
                                {
                                    using (var cnmtStream = cnmtFile.AsStream())
                                    {
                                        var cnmt = new Cnmt(cnmtStream);

                                        nspInfo.CnmtInfo = new CnmtInfo
                                        {
                                            Type = cnmt.Type,
                                            TitleId = cnmt.TitleId.ToHex(),
                                            TitleVersion = cnmt.TitleVersion.Version,
                                        };

                                        //TODO: retrieve the commented info
                                        //cnmt.MinimumSystemVersion
                                        //cnmt.MinimumApplicationVersion

                                        var controlEntries = cnmt.ContentEntries.Where(entry => entry.Type == CnmtContentType.Control).ToArray();
                                        if (controlEntries.Length > 1)
                                            throw new Exception($"More than one NCA of content type \"{CnmtContentType.Control}\" referenced in CNMT file.");

                                        if (controlEntries.Length == 1)
                                        {
                                            var ncaControlFileName = ByteArrayExt.ToHexString(controlEntries[0].NcaId) + ".nca";

                                            var ncaControlFileEntries = nspPartition.Files.Where(entry => string.Equals(entry.Name, ncaControlFileName, StringComparison.InvariantCultureIgnoreCase)).ToArray();
                                            if (ncaControlFileEntries.Length < 1)
                                                throw new Exception($"NCA \"{ncaControlFileName}\" of content type \"{CnmtContentType.Control}\" couldn't be found.");
                                            if (ncaControlFileEntries.Length > 1)
                                                throw new Exception($"NCA \"{ncaControlFileName}\" of content type \"{CnmtContentType.Control}\" found more than once.");

                                            var ncaControlFileEntry = ncaControlFileEntries[0];

                                            using (var ncaControlFile = nspPartition.OpenFile(ncaControlFileEntry, OpenMode.Read))
                                            {
                                                var ncaControl = new Nca(_keyset, new FileStorage(ncaControlFile));

                                                if (ncaControl.Header.ContentType != ContentType.Control)
                                                    throw new Exception($"NCA \"{ncaControlFileName}\" is not of the expected content type \"{CnmtContentType.Control}\".");

                                                var controlFs = ncaControl.OpenFileSystem(ncaControl.Header.ContentIndex, IntegrityCheckLevel.None);

                                                ExtractControlInfo(controlFs, nspInfo);

                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        pfsFile = new PfsFile();
                    }

                    pfsFile.Name = nspFileEntry.Name;
                    pfsFile.Size = nspFileEntry.Size;

                    files.Add(pfsFile);
                }

                nspInfo.Files = files.ToArray();
                return nspInfo;
            }

        }

        private List<ControlInfo> LoadControlsInfo(IEnumerable<CnmtInfo> cnmtsInfo, IReadOnlyCollection<OpenedNca> openedNcas)
        {

            var controlsInfo = new List<ControlInfo>();

            foreach (var cnmtInfo in cnmtsInfo)
            {
                var linkedNcaControlId = cnmtInfo.LinkedNcaControlId;
                if (string.IsNullOrEmpty(linkedNcaControlId))
                    continue;

                var expectedNcaFileName = linkedNcaControlId + ".nca";

                var matchingOpenedNcas = openedNcas.Where(openedNca => string.Equals(expectedNcaFileName, openedNca.FileName, StringComparison.InvariantCultureIgnoreCase)).ToArray();
                if (matchingOpenedNcas.Length < 1)
                {
                    _log?.Error($"CNMT \"{cnmtInfo.FilePath}\" is referencing missing NCA \"{expectedNcaFileName}\" of content type \"{ContentType.Control}\".");
                    continue;
                }

                if (matchingOpenedNcas.Length > 1)
                    _log?.Error($"NCA \"{expectedNcaFileName}\" of content type \"{ContentType.Control}\" was found more than once (found \"{matchingOpenedNcas.Length}\" times), only the first one will be considered.");

                var controlOpenedNca = matchingOpenedNcas[0];
                var controlNca = controlOpenedNca.Nca;
                var controlNcaHeader = controlNca.Header;
                if (controlNcaHeader.ContentType != ContentType.Control)
                {
                    _log?.Error($"NCA \"{controlOpenedNca.FileName}\" is not of the expected content type \"{ContentType.Control}\".");
                    continue;
                }

                //TODO: to finish
                var controlFs = controlNca.OpenFileSystem(controlNcaHeader.ContentIndex, IntegrityCheckLevel.None);

                //ExtractControlInfo(controlFs, nspInfo);

            }

            return controlsInfo;
        }

        private List<CnmtInfo> LoadCnmtsInfo(IEnumerable<OpenedNca> openedNcas)
        {
            //TODO: log warnings for meta nca not ending with cnmt.nca?

            var cnmtsInfo = new List<CnmtInfo>();

            foreach (var openedNca in openedNcas)
            {
                if (!openedNca.FileName.EndsWith(".cnmt.nca", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                if (openedNca.Nca.Header.ContentType != ContentType.Meta)
                {
                    _log?.Error($"Invalid NCA file \"{openedNca.FileName}\", content is not of the expected type \"{ContentType.Meta}\" (found \"{openedNca.Nca.Header.ContentType}\").");
                    continue;
                }

                var cnmtFs = openedNca.Nca.OpenFileSystem(openedNca.Nca.Header.ContentIndex, IntegrityCheckLevel.None);

                var cnmtFileEntries = cnmtFs.OpenDirectory("/", OpenDirectoryMode.Files).Read().Where(entry => (entry.Name ?? "").EndsWith(".cnmt", StringComparison.InvariantCultureIgnoreCase)).ToArray();

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

                        var linkedNcaControlIds = cnmt.ContentEntries.Where(entry => entry.Type == CnmtContentType.Control).Select(entry => ByteArrayExt.ToHexString(entry.NcaId)).ToArray();

                        string linkedNcaControlId;
                        if (linkedNcaControlIds.Length > 1)
                        {
                            _log?.Error($"CNMT file \"{cnmtFileEntry.FullPath}\" contained in NCA \"{openedNca.FileName}\" was not expected to reference more than one NCA of content type \"{CnmtContentType.Control}\". Only the first reference will be considered.");
                            linkedNcaControlId = linkedNcaControlIds[0];
                        }
                        else
                        {
                            linkedNcaControlId = linkedNcaControlIds.Length == 1 ? linkedNcaControlIds[0] : null;
                        }

                        cnmtsInfo.Add(new CnmtInfo
                        {
                            FilePath = cnmtFileEntry.FullPath,
                            Type = cnmt.Type,
                            TitleId = cnmt.TitleId.ToHex(),
                            TitleVersion = cnmt.TitleVersion.Version,
                            LinkedNcaControlId = linkedNcaControlId,
                        });
                    }
                }
            }

            return cnmtsInfo;
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

        private static List<PfsFile> GetPfsFilesInfo(PartitionFileSystem partition, Keyset keyset, out List<OpenedNca> openedNcas)
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
        /// From the given RomFS section of the NCA of content type "Control", retrieves:
        /// -> Info from "control.nacp" (like titles, display version, etc.)
        /// -> Localized title icons
        /// </summary>
        /// <param name="controlFs"></param>
        /// <param name="nspInfo"></param>
        private static void ExtractControlInfo(IFileSystem controlFs, NspInfo nspInfo)
        {
            NacpInfo nacpInfo = null;
            var icons = new List<IconInfo>();
            const string ICON_FILE_PREFIX = "icon_";
            const string ICON_FILE_EXT = ".dat";

            foreach (var controlFileEntry in controlFs.OpenDirectory("/", OpenDirectoryMode.Files).Read())
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
                            nacpInfo = new NacpInfo
                            {
                                DisplayVersion = nacp.DisplayVersion,
                                Titles = titleInfos,
                            };


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
                        }
                    }
                }
                else if (fileName.StartsWith(ICON_FILE_PREFIX, StringComparison.InvariantCultureIgnoreCase) && fileName.EndsWith(ICON_FILE_EXT, StringComparison.InvariantCultureIgnoreCase))
                {
                    var langName = fileName.Substring(ICON_FILE_PREFIX.Length, fileName.Length - ICON_FILE_PREFIX.Length - ICON_FILE_EXT.Length);
                    if (!Enum.TryParse(langName, true, out NacpLanguage language))
                    {
                        Logger.LogWarning($"Found a *.dat file \"{fileName}\" which doesn't match any of the languages.");
                        continue;
                    }

                    var bitmapSource = new BitmapImage();

                    using (var iconFile = controlFs.OpenFile(controlFileEntry.FullPath, OpenMode.Read))
                    {
                        using (var iconStream = iconFile.AsStream())
                        {
                            bitmapSource.BeginInit();
                            bitmapSource.StreamSource = iconStream;
                            bitmapSource.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapSource.EndInit();
                            bitmapSource.Freeze();
                        }
                    }

                    var iconInfo = new IconInfo
                    {
                        Image = bitmapSource,
                        Language = language,
                    };

                    icons.Add(iconInfo);
                }
            }

            nspInfo.NacpInfo = nacpInfo;
            nspInfo.Icons = icons;

        }

    }
}