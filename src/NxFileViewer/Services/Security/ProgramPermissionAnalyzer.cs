using System;
using System.Collections.Generic;
using System.Linq;

namespace Emignatik.NxFileViewer.Services.Security;

public static class ProgramPermissionAnalyzer
{
    public static bool HasFileSystemServices(IEnumerable<string> services) => services.Any(service =>
        service == "*" || service.StartsWith("fsp-", StringComparison.Ordinal) ||
        (service.EndsWith("*", StringComparison.Ordinal) && "fsp-srv".StartsWith(service.TrimEnd('*'), StringComparison.Ordinal)));

    private const ulong PrivilegedFileSystemPermissionFlag = 0x8000000000000000;

    public static ProgramSecurityLevel Analyze(ulong permissionsBitmask, bool hasFileSystemServices = true)
    {
        if (!hasFileSystemServices) return ProgramSecurityLevel.Safe;
        if (permissionsBitmask == ulong.MaxValue)
            return ProgramSecurityLevel.Dangerous;

        return (permissionsBitmask & PrivilegedFileSystemPermissionFlag) != 0
            ? ProgramSecurityLevel.Unsafe
            : ProgramSecurityLevel.Safe;
    }
}

public enum ProgramSecurityLevel
{
    Safe,
    Unsafe,
    Dangerous,
}
