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


namespace Emignatik.NxFileViewer.NSP
{
    public class NspInfoLoader
    {
        private readonly TempDirMgr _tempDirMgr;
        private readonly Keyset _keyset;
        private const string CNMT_NCA = ".cnmt.nca";

        public NspInfoLoader(TempDirMgr tempDirMgr, Keyset keyset)
        {
            _tempDirMgr = tempDirMgr ?? throw new ArgumentNullException(nameof(tempDirMgr));
            _keyset = keyset ?? throw new ArgumentNullException(nameof(keyset));
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

                var files = new List<PfsFile>();
                var metaFiles = new List<PfsNcaFile>();
                foreach (var nspFileEntry in nspPartition.Files)
                {
                    var fileName = nspFileEntry.Name ?? "";

                    PfsFile pfsFile;
                    if (fileName.EndsWith(".nca", StringComparison.InvariantCultureIgnoreCase))
                    {
                        using (var openFile = nspPartition.OpenFile(nspFileEntry, OpenMode.Read))
                        {
                            var nca = new Nca(_keyset, new FileStorage(openFile));
                            var pfsNcaFile = new PfsNcaFile
                            {
                                Name = nspFileEntry.Name,
                                ContentType = nca.Header.ContentType,
                                TitleId = nca.Header.TitleId,
                                SdkVersion = nca.Header.SdkVersion.ToString()

                            };
                            pfsFile = pfsNcaFile;

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

                            if (fileName.EndsWith(CNMT_NCA, StringComparison.InvariantCultureIgnoreCase))
                            {
                                if (nca.Header.ContentType != ContentType.Meta)
                                    throw new Exception($"\"{fileName}\" is not of the expected type \"{ContentType.Meta}\"");

                                var cnmtFs = nca.OpenFileSystem(nca.Header.ContentIndex, IntegrityCheckLevel.None);

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
                                        var controlEntries = cnmt.ContentEntries.Where(entry => entry.Type == LibHac.CnmtContentType.Control).ToArray();
                                        if (controlEntries.Length < 1)
                                            throw new Exception($"No NCA of content type \"{CnmtContentType.Control}\" referenced in CNMT file.");
                                        if (controlEntries.Length > 1)
                                            throw new Exception($"More than one NCA of content type \"{CnmtContentType.Control}\" referenced in CNMT file.");

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
                                metaFiles.Add(pfsNcaFile);
                            }
                        }
                    }
                    else
                    {
                        pfsFile = new PfsFile();
                    }

                    files.Add(pfsFile);
                    pfsFile.Name = nspFileEntry.Name;
                    pfsFile.Size = nspFileEntry.Size;

                }

                nspInfo.Files = files.ToArray();
                return nspInfo;
            }

        }

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