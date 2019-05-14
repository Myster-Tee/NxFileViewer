using System.Runtime.InteropServices;
using System.Text;

namespace Emignatik.NxFileViewer.NxFormats.NCA.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NcaSuperBlockRomFsStruct
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] MagicRaw;

        public string Magic
        {
            get
            {
                var magicRaw = MagicRaw;
                return magicRaw == null ? null : Encoding.ASCII.GetString(magicRaw);
            }
        }

        [MarshalAs(UnmanagedType.U4)]
        public uint MagicNum;

        [MarshalAs(UnmanagedType.U4)]
        public uint MasterHashSize;

        [MarshalAs(UnmanagedType.U4)]
        public uint Unknown1;

        [MarshalAs(UnmanagedType.U8)]
        public ulong Level1Offset;

        [MarshalAs(UnmanagedType.U8)]
        public ulong Level1Size;

        [MarshalAs(UnmanagedType.U4)]
        public uint Level1BlockSize;

        [MarshalAs(UnmanagedType.U4)]
        public uint Level1Reserved;

        [MarshalAs(UnmanagedType.U8)]
        public ulong Level2Offset;

        [MarshalAs(UnmanagedType.U8)]
        public ulong Level2Size;

        [MarshalAs(UnmanagedType.U4)]
        public uint Level2BlockSize;

        [MarshalAs(UnmanagedType.U4)]
        public uint Level2Reserved;

        [MarshalAs(UnmanagedType.U8)]
        public ulong Level3Offset;

        [MarshalAs(UnmanagedType.U8)]
        public ulong Level3Size;

        [MarshalAs(UnmanagedType.U4)]
        public uint Level3BlockSize;

        [MarshalAs(UnmanagedType.U4)]
        public uint Level3Reserved;

        [MarshalAs(UnmanagedType.U8)]
        public ulong Level4Offset;

        [MarshalAs(UnmanagedType.U8)]
        public ulong Level4Size;

        [MarshalAs(UnmanagedType.U4)]
        public uint Level4BlockSize;

        [MarshalAs(UnmanagedType.U4)]
        public uint Level4Reserved;

        [MarshalAs(UnmanagedType.U8)]
        public ulong Level5Offset;

        [MarshalAs(UnmanagedType.U8)]
        public ulong Level5Size;

        [MarshalAs(UnmanagedType.U4)]
        public uint Level5BlockSize;

        [MarshalAs(UnmanagedType.U4)]
        public uint Level5Reserved;

        [MarshalAs(UnmanagedType.U8)]
        public ulong Level6Offset;

        [MarshalAs(UnmanagedType.U8)]
        public ulong Level6Size;

        [MarshalAs(UnmanagedType.U4)]
        public uint Level6BlockSize;

        [MarshalAs(UnmanagedType.U4)]
        public uint Level6Reserved;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] Unknown2;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] Hash;
    }

}
