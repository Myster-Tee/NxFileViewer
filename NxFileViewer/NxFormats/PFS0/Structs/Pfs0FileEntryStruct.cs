using System.Runtime.InteropServices;

namespace Emignatik.NxFileViewer.NxFormats.PFS0.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Pfs0FileEntryStruct
    {
        /// <summary>
        /// Offset of file in Data
        /// </summary>
        [MarshalAs(UnmanagedType.U8)]
        public ulong FileOffset;

        /// <summary>
        /// Size of file in Data
        /// </summary>
        [MarshalAs(UnmanagedType.U8)]
        public ulong FileSize;

        /// <summary>
        /// Offset of filename in String Table 
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint FileNameOffset;

        /// <summary>
        /// Normally zero?
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint Reserved;
    }
}