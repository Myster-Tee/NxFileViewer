namespace Emignatik.NxFileViewer.NxFormats.CNMT.Structs
{
    public enum CnmtContentType : byte
    {
        Meta = 0x00,

        Program = 0x01,

        Data = 0x02,

        Control = 0x03,

        HtmlDocument = 0x04,

        LegalInformation = 0x05,

        DeltaFragment = 0x06
    }
}
