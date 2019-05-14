using Emignatik.NxFileViewer.NxFormats.NACP.Structs;
using Emignatik.NxFileViewer.Utils;

namespace Emignatik.NxFileViewer.NxFormats.NACP.Models
{
    /// <summary>
    /// NACP file data model
    /// (aggregates raw structures and shortcuts some interesting information)
    /// </summary>
    public class NacpContent
    {
        public NacpStruct RawStruct { get; internal set; }

        public NacpTitle[] Titles { get; internal set; }

        /// <summary>
        /// Human readable version of the application
        /// </summary>
        public string DisplayVersion => RawStruct.DisplayVersion.AsNullTerminatedString();

        public string Isbn => RawStruct.Isbn.AsNullTerminatedString();

        /// <summary>
        /// Seems to be the same as the title id? TODO: to be verified
        /// </summary>
        public string PresenceGroupId => RawStruct.PresenceGroupId.ToHex();

        public string AddOnContentBaseId => RawStruct.AddOnContentBaseId.ToHex();

        public string SaveDataOwnerId => RawStruct.SaveDataOwnerId.ToHex();

        public string BcatPassphrase => RawStruct.BcatPassphrase.AsNullTerminatedString();

    }
}
