namespace Emignatik.NxFileViewer.NxFormats.CNMT.Structs
{
    /// <summary>
    /// https://switchbrew.org/wiki/NCM_services#Title_Types
    /// </summary>
    public enum CnmtType : byte
    {
        /// <summary>
        /// System Modules or System Applets
        /// </summary>
        SystemProgram = 0x01,

        /// <summary>
        /// (System Data Archives)
        /// </summary>
        SystemData = 0x02,

        SystemUpdate = 0x03,

        /// <summary>
        /// (Firmware package A or C)
        /// </summary>
        BootImagePackage = 0x04,

        /// <summary>
        /// (Firmware package B or D)
        /// </summary>
        BootImagePackageSafe = 0x05,

        Application = 0x80,

        Patch = 0x81,

        AddOnContent = 0x82,

        Delta = 0x83,

    }
}