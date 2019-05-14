using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Emignatik.NxFileViewer.KeysParsing;
using Emignatik.NxFileViewer.NxFormats.NCA;
using Emignatik.NxFileViewer.NxFormats.PFS0.Models;
using Emignatik.NxFileViewer.Utils;

namespace Emignatik.NxFileViewer.NxFormats.PFS0
{
    public static class Pfs0Utils
    {

        /// <summary>
        /// Extracts files embedded in a PFS0 file
        /// </summary>
        /// <param name="pfs0FilePath">The path to the input PFS0 file</param>
        /// <param name="fileNames">The list of file names to extract</param>
        /// <param name="targetDirectory">The target directory</param>
        public static void SavePfs0Files(string pfs0FilePath, IEnumerable<string> fileNames, string targetDirectory)
        {
            using (var pfs0Reader = Pfs0Reader.FromFile(pfs0FilePath))
            {
                var fileDefinitions = ListCorrespondingFileDefinitions(fileNames, pfs0Reader.ReadFileDefinitions(), pfs0FilePath);

                foreach (var fileDefinition in fileDefinitions)
                {
                    using (var srcStream = pfs0Reader.GetFileStream(fileDefinition))
                    {
                        using (var dstStream = File.Create(Path.Combine(targetDirectory, fileDefinition.FileName)))
                        {
                            srcStream.CopyTo(dstStream);
                        }
                    }
                }
            }
        }

        public static void DecryptPfs0NcaFilesHeader(string pfs0FilePath, IEnumerable<string> fileNames, string targetDirectory, KeySet keySet, bool checkFilesExt = true)
        {
            if (pfs0FilePath == null) throw new ArgumentNullException(nameof(pfs0FilePath));
            if (fileNames == null) throw new ArgumentNullException(nameof(fileNames));
            if (targetDirectory == null) throw new ArgumentNullException(nameof(targetDirectory));
            if (keySet == null) throw new ArgumentNullException(nameof(keySet));

            if (checkFilesExt)
            {
                foreach (var fileName in fileNames)
                {
                    if (!string.Equals(".nca", Path.GetExtension(fileName), StringComparison.InvariantCultureIgnoreCase))
                        throw new ArgumentException($"Can't decrypt file \"{fileName}\", not an NCA (*.nca).");
                }
            }

            using (var pfs0Reader = Pfs0Reader.FromFile(pfs0FilePath))
            {
                var fileDefinitions = ListCorrespondingFileDefinitions(fileNames, pfs0Reader.ReadFileDefinitions(), pfs0FilePath);

                foreach (var fileDefinition in fileDefinitions)
                {
                    using (var srcStream = new NcaDecryptionStream(pfs0Reader.GetFileStream(fileDefinition), keySet))
                    {
                        var decryptedHeaders = srcStream.ReadBytes(NcaReader.ALL_HEADERS_SIZE);

                        var saveFileName = $"{Path.GetFileNameWithoutExtension(fileDefinition.FileName)}.header{Path.GetExtension(fileDefinition.FileName)}";

                        var filePath = Path.Combine(targetDirectory, saveFileName);
                        File.WriteAllBytes(filePath, decryptedHeaders);
                    }
                }
            }
        }

        private static List<Pfs0FileDefinition> ListCorrespondingFileDefinitions(IEnumerable<string> fileNames, Pfs0FileDefinition[] fileDefinitions, string pfs0FilePath)
        {
            var fileDefinitionsToSave = new List<Pfs0FileDefinition>();
            foreach (var fileName in fileNames)
            {
                var fileDefinition = fileDefinitions.FirstOrDefault(fd => fd?.FileName == fileName);
                if (fileDefinition == null)
                    throw new ArgumentException($"File \"{fileName}\" not found in \"{pfs0FilePath}\".");
                fileDefinitionsToSave.Add(fileDefinition);
            }
            return fileDefinitionsToSave;
        }
    }
}
