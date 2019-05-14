using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Emignatik.NxFileViewer.Logging;
using Emignatik.NxFileViewer.NxFormats.PFS0.Models;
using Emignatik.NxFileViewer.NxFormats.PFS0.Structs;
using Emignatik.NxFileViewer.Utils;

namespace Emignatik.NxFileViewer.NxFormats.PFS0
{

    /// <inheritdoc />
    /// <summary>
    /// A PFS0 file is nothing more than a files container.
    /// 
    /// Based on SwitchBrew specification here: "https://switchbrew.org/wiki/NCA_Format#PFS0"
    /// and here "https://wiki.oatmealdome.me/PFS0_(File_Format)"
    /// </summary>
    public class Pfs0Reader : IDisposable
    {
        private readonly Stream _stream;
        private Pfs0HeaderStruct? _headerStruct = null;
        private SubStream<Pfs0FileDefinition> _fileStream;

        public Pfs0Reader(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public static bool IsPfs0(string filePath)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));

            if (!File.Exists(filePath)) throw new Exception($"File \"{filePath}\" not found!");

            using (var fileStream = File.OpenRead(filePath))
            {
                var buff = new byte[4];
                fileStream.Read(buff, 0, buff.Length);
                return string.Equals("PFS0", Encoding.ASCII.GetString(buff));
            }
        }

        public Pfs0HeaderStruct ReadHeader(bool force = false)
        {
            if (_headerStruct == null || force)
            {
                _stream.Position = 0;
                _headerStruct = _stream.ReadStruct<Pfs0HeaderStruct>();
                return _headerStruct.Value;
            }

            _stream.Position = Marshal.SizeOf(typeof(Pfs0HeaderStruct));
            return _headerStruct.Value;
        }

        public Pfs0FileDefinition[] ReadFileDefinitions()
        {
            var headerStruct = ReadHeader();

            var fileNamesStartPos = _stream.Position + headerStruct.NbFiles * Marshal.SizeOf(typeof(Pfs0FileEntryStruct));
            var filesContentStartPos = (ulong)fileNamesStartPos + headerStruct.FileNamesTableSize;

            var fileDefinitions = new Pfs0FileDefinition[headerStruct.NbFiles];
            for (var fileIndex = 0; fileIndex < headerStruct.NbFiles; fileIndex++)
            {
                var fileEntryStruct = _stream.ReadStruct<Pfs0FileEntryStruct>();

                var posBackup = _stream.Position;

                _stream.Position = fileNamesStartPos + fileEntryStruct.FileNameOffset;

                fileDefinitions[fileIndex] = new Pfs0FileDefinition
                {
                    RawStruct = fileEntryStruct,
                    FileName = _stream.ReadNullTerminatedString(),
                    FileStartPos = filesContentStartPos + fileEntryStruct.FileOffset,
                    FileSize = fileEntryStruct.FileSize,
                };

                _stream.Position = posBackup;
            }

            return fileDefinitions;
        }

        public SubStream GetFileStream(Pfs0FileDefinition fileDefinition)
        {
            if (fileDefinition == null)
                throw new ArgumentNullException(nameof(fileDefinition));

            if (_fileStream != null) //NOTE: as SubStream is just a wrapper to the base stream it is important to prevent multiple access
                throw new InvalidOperationException($"Only one \"{nameof(SubStream)}\" can be obtained at a time. Close \"{fileDefinition.FileName}\" and try again.");

            _fileStream = new SubStream<Pfs0FileDefinition>(_stream, (long)fileDefinition.FileStartPos, (long)fileDefinition.FileSize)
            {
                AttachedData = fileDefinition,
            };
            _fileStream.Disposed += (s, a) => { _fileStream = null; };

            return _fileStream;
        }

        public void SaveFile(Pfs0FileDefinition fileDefinition, string filePath)
        {
            if (fileDefinition == null)
                throw new ArgumentNullException(nameof(fileDefinition));

            using (var pfs0FileStream = GetFileStream(fileDefinition))
            {
                using (var fileStream = File.Create(filePath))
                {
                    pfs0FileStream.CopyTo(fileStream);
                }
            }
        }

        public void SaveFileToDir(Pfs0FileDefinition fileDefinition, string dirPath, out string filePath)
        {
            if (fileDefinition == null)
                throw new ArgumentNullException(nameof(fileDefinition));

            var fileName = IoHelper.SanitizeFileName(fileDefinition.FileName);

            filePath = Path.GetFullPath(Path.Combine(dirPath, fileName));

            SaveFile(fileDefinition, filePath);
        }

        public void Dispose()
        {
            try
            {
                _stream.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to dispose PFS0 stream.", ex);
            }
        }


        public static Pfs0Reader FromFile(string filePath)
        {
            return new Pfs0Reader(File.OpenRead(filePath));
        }

    }
}