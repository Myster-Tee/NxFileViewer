using Emignatik.NxFileViewer.NxFormats.CNMT.Structs;
using Emignatik.NxFileViewer.Utils;

namespace Emignatik.NxFileViewer.NxFormats.CNMT.Models
{
    /// <summary>
    /// Represents of record
    /// </summary>
    public class CnmtContentRecord
    {

        public CnmtContentRecordStruct RawStruct { get; internal set; } //TODO: harmonize internal vs wrapper

        public string NcaId => RawStruct.NcaId.ToHexString(lowerCase: true);

        public CnmtContentType Type => RawStruct.ContentType;

        public override string ToString()
        {
            return $"{NcaId} ({Type})";
        }
    }
}