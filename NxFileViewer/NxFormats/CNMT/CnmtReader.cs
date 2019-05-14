using System;
using System.IO;
using System.Runtime.InteropServices;
using Emignatik.NxFileViewer.NxFormats.CNMT.Models;
using Emignatik.NxFileViewer.NxFormats.CNMT.Structs;
using Emignatik.NxFileViewer.Utils;

namespace Emignatik.NxFileViewer.NxFormats.CNMT
{

    /// <inheritdoc />
    /// <summary>
    /// A CNMT file is a metadata file.
    /// It contains various information about a package (like NSP).
    /// 
    /// Based on SwitchBrew specification here: "https://switchbrew.org/wiki/CNMT"
    /// </summary>
    public class CnmtReader : IDisposable
    {
        private readonly Stream _stream;

        public CnmtReader(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        public CnmtHeader Read()
        {
            _stream.Position = 0;

            var cnmtHeaderStruct = _stream.ReadStruct<CnmtHeaderStruct>();
            var cnmtHeader = new CnmtHeader
            {
                RawStruct = cnmtHeaderStruct,
            };

            switch (cnmtHeaderStruct.Type)
            {
                case CnmtType.SystemProgram:
                    break;
                case CnmtType.SystemData:
                    break;
                case CnmtType.SystemUpdate:
                    break;
                case CnmtType.BootImagePackage:
                    break;
                case CnmtType.BootImagePackageSafe:
                    break;
                case CnmtType.Application:
                    cnmtHeader.ExtendedRawStruct = _stream.ReadStruct<CnmtAppHeaderStruct>();
                    break;
                case CnmtType.Patch:
                    cnmtHeader.ExtendedRawStruct = _stream.ReadStruct<CnmtPatchHeaderStruct>();
                    break;
                case CnmtType.AddOnContent:
                    cnmtHeader.ExtendedRawStruct = _stream.ReadStruct<CnmtAddonContentHeaderStruct>();
                    break;
                case CnmtType.Delta:
                    break;
                default:
                    //TODO: maybe not throw?
                    throw new ArgumentOutOfRangeException();
            }

            if (cnmtHeaderStruct.NbContentRecords > 0 && cnmtHeaderStruct.NbMetaRecords > 0)
            {
                //TODO: should throw? Can this case happen? If yes, which records are first, meta or content?
            }
            else if (cnmtHeaderStruct.NbContentRecords > 0)
            {
                _stream.Position = Marshal.SizeOf<CnmtHeaderStruct>() + cnmtHeaderStruct.TableOffset;

                var cnmtContentRecords = new CnmtContentRecord[cnmtHeaderStruct.NbContentRecords];
                for (var i = 0; i < cnmtHeaderStruct.NbContentRecords; i++)
                {
                    var contentRecordStruct = _stream.ReadStruct<CnmtContentRecordStruct>();
                    cnmtContentRecords[i] = new CnmtContentRecord
                    {
                        RawStruct = contentRecordStruct,
                    };
                }

                cnmtHeader.ContentRecords = cnmtContentRecords;
            }
            else if (cnmtHeaderStruct.NbMetaRecords > 0)
            {
                //Not implemented
            }

            return cnmtHeader;
        }

        public static CnmtReader FromFile(string filePath)
        {
            return new CnmtReader(File.OpenRead(filePath));
        }

    }
}
