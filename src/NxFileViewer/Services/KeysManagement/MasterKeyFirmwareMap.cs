using System.Collections.Generic;

namespace Emignatik.NxFileViewer.Services.KeysManagement;

public static class MasterKeyFirmwareMap
{
    private static readonly IReadOnlyList<string> FirmwareVersions =
    [
        "1.0.0–2.3.0",
        "3.0.0",
        "3.0.1",
        "4.0.0",
        "5.0.0",
        "6.0.0",
        "6.2.0",
        "7.0.0",
        "8.1.0",
        "9.0.0",
        "9.1.0",
        "12.1.0",
        "13.0.0",
        "14.0.0",
        "15.0.0",
        "16.0.0",
        "17.0.0",
        "18.0.0",
        "19.0.0",
        "20.0.0",
        "21.0.0",
        "22.0.0",
        "23.0.0",
    ];

    public static string? GetSupportedFirmware(int masterKeyRevision) =>
        masterKeyRevision >= 0 && masterKeyRevision < FirmwareVersions.Count
            ? FirmwareVersions[masterKeyRevision]
            : null;
}
