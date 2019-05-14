using System.Runtime.InteropServices;

namespace Emignatik.NxFileViewer.NxFormats.NCA.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NcaSectionTableEntryStruct
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint MediaOffset;

        [MarshalAs(UnmanagedType.U4)]
        public uint MediaEndOffset;

        [MarshalAs(UnmanagedType.U4)]
        public uint Unknown1;

        [MarshalAs(UnmanagedType.U4)]
        public uint Unknown2;
    }
}
