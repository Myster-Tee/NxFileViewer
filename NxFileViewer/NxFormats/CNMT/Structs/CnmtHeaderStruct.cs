using System.Runtime.InteropServices;

namespace Emignatik.NxFileViewer.NxFormats.CNMT.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CnmtHeaderStruct
    {
        [MarshalAs(UnmanagedType.U8)]
        public ulong TitleId;

        [MarshalAs(UnmanagedType.U4)]
        public uint TitleVersion;

        [MarshalAs(UnmanagedType.U1)]
        public CnmtType Type;

        [MarshalAs(UnmanagedType.U1)]
        public byte Unknown1;

        /// <summary>
        /// Offset to table relative to the end of this 0x20-byte header.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)]
        public ushort TableOffset;

        /// <summary>
        /// Number of content records
        /// (Each record seems to be a reference to an NCA file in the NSP, but all NCA files in NSP are not necessarily referenced)
        /// </summary>
        [MarshalAs(UnmanagedType.U2)]
        public ushort NbContentRecords;

        /// <summary>
        /// Number of meta records
        /// </summary>
        [MarshalAs(UnmanagedType.U2)]
        public ushort NbMetaRecords;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x0C)]
        public byte[] Unknown2;

    }
}
