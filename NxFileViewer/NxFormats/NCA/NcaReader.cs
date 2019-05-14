using System;
using System.IO;
using Emignatik.NxFileViewer.Logging;
using Emignatik.NxFileViewer.NxFormats.NCA.Models;
using Emignatik.NxFileViewer.NxFormats.NCA.Structs;
using Emignatik.NxFileViewer.NxFormats.PFS0;
using Emignatik.NxFileViewer.Utils;

namespace Emignatik.NxFileViewer.NxFormats.NCA
{

    /// <inheritdoc />
    /// <summary>
    /// A NCA file is a container which can contain up to 4 sections.
    /// 
    /// Each section can be either a PFS0 (<see cref="Pfs0Reader" />) or a RomFS.
    /// 
    /// Based on SwitchBrew specification here: "https://switchbrew.org/wiki/NCA_Format"
    /// </summary>
    public class NcaReader : IDisposable
    {
        public const int HEADER_SIZE = 0x400;
        public const int HEADER_SECTION_SIZE = 0x200;
        public const int NB_HEADER_SECTIONS = 0x04;
        public const int ALL_HEADERS_SIZE = HEADER_SIZE + NB_HEADER_SECTIONS * HEADER_SECTION_SIZE;


        private readonly Stream _stream;
        private NcaHeader _ncaHeader;

        public NcaReader(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        /// <summary>
        /// Decrypt and read the NCA header.
        /// </summary>
        /// <param name="force">True to force reading the header again (discard cached header)</param>
        /// <returns></returns>
        public NcaHeader ReadHeader(bool force = false)
        {
            if (_ncaHeader != null && !force) return _ncaHeader;

            var headerStruct = _stream.ReadStruct<NcaHeaderStruct>(0);
            var ncaHeader = new NcaHeader
            {
                RawStruct = headerStruct,
                Sections = new NcaSectionHeader[4],
            };

            var magic = headerStruct.Magic;
            var isNca2 = magic == "NCA2";
            var isNca3 = !isNca2 && magic == "NCA3";

            if (!isNca2 && !isNca3) throw new Exception($"File is not a valid NCA (Magic was \"{magic}\")!");

            // Set the stream position to the beginning of header sections
            _stream.Position = HEADER_SIZE;

            // Read and decrypt sections headers
            for (var sectionIndex = 0; sectionIndex < NB_HEADER_SECTIONS; sectionIndex++)
            {
                var sectionHeaderBytes = _stream.ReadBytes(HEADER_SECTION_SIZE);

                var sectionHeaderStruct = sectionHeaderBytes.ToStruct<NcaSectionHeaderWithoutSuperBlockStruct>();

                if (sectionHeaderStruct.IsEmpty) continue;

                object superBlock;
                const int SUPER_BLOCK_OFFSET = 0x8;
                switch (sectionHeaderStruct.FileSystemType)
                {
                    case NcaFileSystemType.PFS0:

                        var ncaSuperBlockPfs0 = sectionHeaderBytes.ToStruct<NcaSuperBlockPfs0Struct>(SUPER_BLOCK_OFFSET);
                        const int EXPECTED_CONTROL = 0x02;
                        if (ncaSuperBlockPfs0.Control != EXPECTED_CONTROL)
                        {
                            // NOTE: Maybe we should be consider an error ?
                            Logger.LogWarning($"Invalid section \"{sectionIndex}\", \"{nameof(ncaSuperBlockPfs0.Control)}\" of \"{sectionHeaderStruct.FileSystemType}\" super block is not equal to \"0x{EXPECTED_CONTROL:X2}\".");
                        }

                        superBlock = ncaSuperBlockPfs0;
                        break;

                    case NcaFileSystemType.ROM_FS:
                        var ncaSuperBlockRomFs = sectionHeaderBytes.ToStruct<NcaSuperBlockRomFsStruct>(SUPER_BLOCK_OFFSET);
                        const string EXPECTED_MAGIC = "IVFC";
                        if (ncaSuperBlockRomFs.Magic != EXPECTED_MAGIC)
                        {
                            // NOTE: Maybe we should be consider an error ?
                            Logger.LogWarning($"Invalid section \"{sectionIndex}\", \"{nameof(ncaSuperBlockRomFs.Magic)}\" of \"{sectionHeaderStruct.FileSystemType}\" super block is not equal to \"{EXPECTED_MAGIC}\".");
                        }

                        const int EXPECTED_MAGIC_NUM = 0x20000;
                        if (ncaSuperBlockRomFs.MagicNum != EXPECTED_MAGIC_NUM)
                        {
                            // NOTE: Maybe we should be consider an error ?
                            Logger.LogWarning($"Invalid section \"{sectionIndex}\", \"{nameof(ncaSuperBlockRomFs.MagicNum)}\" of \"{sectionHeaderStruct.FileSystemType}\" super block is not equal to \"0x{EXPECTED_MAGIC_NUM:X2}\".");
                        }

                        superBlock = ncaSuperBlockRomFs;
                        break;
                    default:
                        // NOTE: Maybe we should be consider an error ?
                        Logger.LogWarning($"Invalid section \"{sectionIndex}\",  \"{sectionHeaderStruct.FileSystemType}\" is unknown (or unsupported).");
                        continue;
                }

                ncaHeader.Sections[sectionIndex] = new NcaSectionHeader
                {
                    SectionIndex = (NcaSectionIndex)sectionIndex,
                    SectionHeaderStruct = sectionHeaderStruct,
                    SuperBlock = superBlock,
                };
            }

            _ncaHeader = ncaHeader;
            return ncaHeader;
        }

        public void Dispose()
        {
            try
            {
                _stream.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to dispose NCA stream.", ex);
            }
        }

        public static NcaReader FromFile(string filePath)
        {
            return new NcaReader(File.OpenRead(filePath));
        }
    }
}
