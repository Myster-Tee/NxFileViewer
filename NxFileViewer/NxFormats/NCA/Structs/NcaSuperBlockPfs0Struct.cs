using System.Runtime.InteropServices;

namespace Emignatik.NxFileViewer.NxFormats.NCA.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NcaSuperBlockPfs0Struct
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] TableHash;

        [MarshalAs(UnmanagedType.U4)]
        public uint BlockSize;

        [MarshalAs(UnmanagedType.U4)]
        public uint Control;

        [MarshalAs(UnmanagedType.U8)]
        public ulong HashTableOffset;

        [MarshalAs(UnmanagedType.U8)]
        public ulong HashTableSize;

        [MarshalAs(UnmanagedType.U8)]
        public ulong SectionStartOffset;

        [MarshalAs(UnmanagedType.U8)]
        public ulong ActualByteSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0xB0)]
        public byte[] Reserved;

    }

}
