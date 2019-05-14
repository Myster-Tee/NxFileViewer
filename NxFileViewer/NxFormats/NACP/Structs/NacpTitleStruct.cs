using System.Runtime.InteropServices;

namespace Emignatik.NxFileViewer.NxFormats.NACP.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NacpTitleStruct
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x200)]
        public byte[] AppName;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x100)]
        public byte[] Publisher;
    }
}