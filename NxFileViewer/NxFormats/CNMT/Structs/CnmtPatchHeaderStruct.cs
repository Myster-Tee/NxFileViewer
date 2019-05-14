using System.Runtime.InteropServices;

namespace Emignatik.NxFileViewer.NxFormats.CNMT.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CnmtPatchHeaderStruct
    {
        [MarshalAs(UnmanagedType.U8)]
        public ulong OriginalTitleId;

        [MarshalAs(UnmanagedType.U8)]
        public ulong MinSystemVersion;
    }
}