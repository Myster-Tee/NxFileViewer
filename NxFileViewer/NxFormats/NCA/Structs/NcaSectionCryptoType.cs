namespace Emignatik.NxFileViewer.NxFormats.NCA.Structs
{
    public enum NcaSectionCryptoType: byte
    {
        INVALID = 0x00,
        PLAINTEXT = 0x01,
        OTHER = 0x02,
        REGULAR = 0x03,
        UNKNOWN = 0x04
    }
}