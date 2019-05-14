using System.Runtime.InteropServices;
using System.Text;

namespace Emignatik.NxFileViewer.NxFormats.NCA.Structs
{
    /// <summary>
    /// https://switchbrew.org/wiki/NCA_Format#Header
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NcaHeaderStruct
    {
        /// <summary>
        /// RSA-2048 signature over the 0x200-bytes starting at offset 0x200 using fixed key.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x100)]
        public byte[] RSA1;

        /// <summary>
        /// RSA-2048 signature over the 0x200-bytes starting at offset 0x200 using key from NPDM, or zeroes if not a program.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x100)]
        public byte[] RSA2;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] MagicRaw;

        /// <summary>
        /// Magicnum (Normally "NCA3", "NCA2" for some pre-1.0.0 NCAs)
        /// </summary>
        public string Magic
        {
            get
            {
                var magicRaw = MagicRaw;
                return magicRaw == null ? null : Encoding.ASCII.GetString(magicRaw);
            }
        }

        /// <summary>
        /// 0 for system NCAs. 1 for a NCA from gamecard.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public NcaType NcaType;

        /// <summary>
        /// Content Type (0=Program, 1=Meta, 2=Control, 3=Manual, 4=Data, 5=PublicData)
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public NcaContentType ContentType;

        /// <summary>
        /// Crypto Type. Only used stating with 3.0.0. Normally 0. 2 = Crypto supported starting with 3.0.0.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public byte CryptoType;

        /// <summary>
        /// Key index, must be 0-2.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public byte KeyIndex;

        /// <summary>
        /// Size of the entire NCA.
        /// </summary>
        [MarshalAs(UnmanagedType.U8)]
        public ulong EntireNcaSize;

        /// <summary>
        /// titleID
        /// </summary>
        [MarshalAs(UnmanagedType.U8)]
        public ulong TitleIdRaw;

        public string TitleId => TitleIdRaw.ToString("X16");

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public byte[] Unknown1;

        /// <summary>
        /// sdk_version. byte0 is normally 0? Compared with a required minimum-value(0x000B0000). Matches this string from codebin: "FS_ACCESS: { sdk_version: {byte3}.{byte2}.{byte1}, ...".
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public byte[] SdkVersionRaw;

        public string SdkVersion
        {
            get
            {
                if (SdkVersionRaw == null || SdkVersionRaw.Length != 4) return "";

                return $"{SdkVersionRaw[3]}.{SdkVersionRaw[2]}.{SdkVersionRaw[1]}.{SdkVersionRaw[0]}";
            }
        }

        /// <summary>
        /// Crypto-Type2. Selects which crypto-sysver to use. 0x3 = 3.0.1, 0x4 = 4.0.0, 0x5 = 5.0.0.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public byte CryptoType2;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[] Unknown2;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x0F)]
        public byte[] RightsId;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x04)]
        public NcaSectionTableEntryStruct[] Sections;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] HeaderSection1Sha256;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] HeaderSection2Sha256;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] HeaderSection3Sha256;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] HeaderSection4Sha256;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[] Key1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[] Key2;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[] Key3;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[] Key4;
    }
}