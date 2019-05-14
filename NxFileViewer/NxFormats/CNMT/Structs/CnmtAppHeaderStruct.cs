using System.Runtime.InteropServices;

namespace Emignatik.NxFileViewer.NxFormats.CNMT.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CnmtAppHeaderStruct
    {
        [MarshalAs(UnmanagedType.U8)]
        public ulong PatchTitleId;

        [MarshalAs(UnmanagedType.U8)]
        public ulong MinSystemVersion;
    }
}