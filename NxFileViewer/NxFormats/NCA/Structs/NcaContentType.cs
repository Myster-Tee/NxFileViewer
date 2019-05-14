namespace Emignatik.NxFileViewer.NxFormats.NCA.Structs
{
    public enum NcaContentType : byte
    {
        PROGRAM = 0x00,
        META = 0x01,
        CONTROL = 0x02,
        MANUAL = 0x03,
        DATA = 0x04,
        PUBLIC_DATA = 0x05,
    }
}