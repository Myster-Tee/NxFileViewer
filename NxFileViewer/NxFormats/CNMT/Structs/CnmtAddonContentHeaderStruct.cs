using System.Runtime.InteropServices;

namespace Emignatik.NxFileViewer.NxFormats.CNMT.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CnmtAddonContentHeaderStruct
    {
        [MarshalAs(UnmanagedType.U8)]
        public ulong AppTitleId;

        [MarshalAs(UnmanagedType.U8)]
        public ulong MinAppVersion;
    }
}