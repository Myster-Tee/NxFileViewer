using System.Runtime.InteropServices;
using System.Text;

namespace Emignatik.NxFileViewer.NxFormats.PFS0.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Pfs0HeaderStruct
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
        public uint NbFiles;

        [MarshalAs(UnmanagedType.U4)]
        public uint FileNamesTableSize;

        [MarshalAs(UnmanagedType.U4)]
        public uint Reserved;
    }
}