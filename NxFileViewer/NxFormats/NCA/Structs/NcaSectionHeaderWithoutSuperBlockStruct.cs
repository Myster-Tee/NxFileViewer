using System.Runtime.InteropServices;

namespace Emignatik.NxFileViewer.NxFormats.NCA.Structs
{
    /// <summary>
    /// The Section Header Block for each section is at absoluteoffset+0x400+(sectionid*0x200),
    /// where sectionid corresponds to the index used with the entry/hash tables.
    /// The total size is 0x200-bytes.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NcaSectionHeaderWithoutSuperBlockStruct
    {
        /// <summary>
        /// Always 0x2
        /// </summary>
        [MarshalAs(UnmanagedType.U2)]
        public ushort Version;

        /// <summary>
        /// Filesystem Type
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public NcaFileSystemType FileSystemType;

        /// <summary>
        /// Hash Type
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public NcaHashType HashType;

        /// <summary>
        /// Crypto type. 0 and >4 are invalid. 1 = none(plaintext from raw NCA). 2 = other crypto. 3 = regular crypto. 4 = unknown.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public NcaSectionCryptoType CryptoType;

        /// <summary>
        /// Maybe a padding?
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public byte Padding;

        /// <summary>
        /// Returns true if section is empty (contains nothing)
        /// </summary>
        public bool IsEmpty => Version == 0 && FileSystemType == 0 && HashType == 0 && CryptoType == 0 && Padding == 0;

    }
}