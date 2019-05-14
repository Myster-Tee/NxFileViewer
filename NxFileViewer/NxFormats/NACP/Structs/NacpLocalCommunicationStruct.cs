using System.Runtime.InteropServices;

namespace Emignatik.NxFileViewer.NxFormats.NACP.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NacpLocalCommunicationStruct
    {
        [MarshalAs(UnmanagedType.U8)]
        public ulong LocalCommunicationRaw;

        public string LocalCommunicationId => LocalCommunicationRaw.ToString("X16");
    }
}