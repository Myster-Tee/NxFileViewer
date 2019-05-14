using System.Runtime.InteropServices;

namespace Emignatik.NxFileViewer.NxFormats.CNMT.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CnmtContentRecordStruct
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] Hash;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] NcaId;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] Size;

        [MarshalAs(UnmanagedType.U1)]
        public CnmtContentType ContentType;

        [MarshalAs(UnmanagedType.U1)]
        public byte Unknown;

    }
}