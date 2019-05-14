using System.Runtime.InteropServices;

namespace Emignatik.NxFileViewer.NxFormats.NACP.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NacpStruct
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public NacpTitleStruct[] Titles;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x25)]
        public byte[] Isbn;

        [MarshalAs(UnmanagedType.U1)]
        public byte StartupUserAccount;

        [MarshalAs(UnmanagedType.U1)]
        public byte Unknown1;

        [MarshalAs(UnmanagedType.U1)]
        public byte Unknown2;

        [MarshalAs(UnmanagedType.U4)]
        public uint ApplicationAttribute;

        [MarshalAs(UnmanagedType.U4)]
        public NacpLanguage SupportedLanguages;

        [MarshalAs(UnmanagedType.U4)]
        public uint ParentalControl;

        [MarshalAs(UnmanagedType.U1)]
        public bool IsScreenshotEnabled;

        [MarshalAs(UnmanagedType.U1)]
        public NacpVideoCaptureMode VideoCaptureMode;

        [MarshalAs(UnmanagedType.U1)]
        public bool IsDataLossConfirmationEnabled;

        [MarshalAs(UnmanagedType.U1)]
        public byte Unknown3;

        [MarshalAs(UnmanagedType.U8)]
        public ulong PresenceGroupId;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] RatingAge;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[] DisplayVersion;

        [MarshalAs(UnmanagedType.U8)]
        public ulong AddOnContentBaseId;

        [MarshalAs(UnmanagedType.U8)]
        public ulong SaveDataOwnerId;

        [MarshalAs(UnmanagedType.U8)]
        public ulong UserAccountSaveDataSize;

        [MarshalAs(UnmanagedType.U8)]
        public ulong UserAccountSaveDataJournalSize;

        [MarshalAs(UnmanagedType.U8)]
        public ulong DeviceSaveDataSize;

        [MarshalAs(UnmanagedType.U8)]
        public ulong DeviceSaveDataJournalSize;

        [MarshalAs(UnmanagedType.U8)]
        public ulong BcatDeliveryCacheStorageSize;

        [MarshalAs(UnmanagedType.U8)]
        public ulong ApplicationErrorCodeCategory;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x8)]
        public NacpLocalCommunicationStruct[] LocalCommunicationStructs;

        [MarshalAs(UnmanagedType.U1)]
        public byte LogoType;

        [MarshalAs(UnmanagedType.U1)]
        public byte LogoHandling;

        [MarshalAs(UnmanagedType.U1)]
        public bool IsRuntimeAddOnContentInstallEnabled;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x03)]
        public byte[] Unknown4;

        [MarshalAs(UnmanagedType.U1)]
        public byte Unknown5;

        [MarshalAs(UnmanagedType.U1)]
        public byte Unknown6;

        [MarshalAs(UnmanagedType.U8)]
        public ulong SeedForPseudoDeviceId;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x41)]
        public byte[] BcatPassphrase;

        [MarshalAs(UnmanagedType.U1)]
        public byte Unknown7;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x06)]
        public byte[] Unknown8;

        [MarshalAs(UnmanagedType.U8)]
        public ulong UserAccountSaveDataMaxSize;

        [MarshalAs(UnmanagedType.U8)]
        public ulong UserAccountSaveDataMaxJournalSize;

        [MarshalAs(UnmanagedType.U8)]
        public ulong DeviceSaveDataMaxSize;

        [MarshalAs(UnmanagedType.U8)]
        public ulong DeviceSaveDataMaxJournalSize;

        [MarshalAs(UnmanagedType.U8)]
        public ulong CacheStorageSize;

        [MarshalAs(UnmanagedType.U8)]
        public ulong CacheStorageJournalSize;

        [MarshalAs(UnmanagedType.U8)]
        public ulong CacheStorageMaxSizeAndMaxJournalSize;

        [MarshalAs(UnmanagedType.U8)]
        public ulong CacheStorageMaxIndex;
    }
}