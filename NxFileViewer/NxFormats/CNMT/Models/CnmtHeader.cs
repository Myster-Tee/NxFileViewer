using Emignatik.NxFileViewer.NxFormats.CNMT.Structs;
using Emignatik.NxFileViewer.Utils;

namespace Emignatik.NxFileViewer.NxFormats.CNMT.Models
{

    /// <summary>
    /// CNMT header data model
    /// (aggregates raw structures and shortcuts some interesting information)
    /// </summary>
    public class CnmtHeader
    {
        public CnmtHeaderStruct RawStruct { get; internal set; }

        public CnmtType Type => RawStruct.Type;

        public string TitleId => RawStruct.TitleId.ToHex();

        public uint TitleVersion => RawStruct.TitleVersion;

        /// <summary>
        /// If <see cref="Type"/> is <see cref="CnmtType.Application"/>, value is <see cref="CnmtAppHeaderStruct"/>
        /// If <see cref="Type"/> is <see cref="CnmtType.Patch"/>, value is <see cref="CnmtPatchHeaderStruct"/>
        /// If <see cref="Type"/> is <see cref="CnmtType.AddOnContent"/>, value is <see cref="CnmtAddonContentHeaderStruct"/>
        /// Otherwise null.
        /// </summary>
        public object ExtendedRawStruct { get; internal set; }

        public CnmtContentRecord[] ContentRecords { get; internal set; }

        public override string ToString()
        {
            return $"{TitleId} - {Type} ({TitleVersion})";
        }
    }
}