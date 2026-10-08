using Emignatik.NxFileViewer.Services.Security;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.Security;

public class ProgramPermissionAnalyzerTest
{
    [Theory]
    [InlineData("fsp-srv", true)]
    [InlineData("fsp-ldr", true)]
    [InlineData("fsp-*", true)]
    [InlineData("*", true)]
    [InlineData("f*", true)]
    [InlineData("fs", false)]
    [InlineData("hid", false)]
    public void ServiceAccessRecognizesFilesystemAndWildcards(string service, bool expected) =>
        Assert.Equal(expected, ProgramPermissionAnalyzer.HasFileSystemServices(new[] { service }));

    [Fact]
    public void PrivilegedMaskWithoutFilesystemServiceAccessIsSafe()
    {
        Assert.Equal(ProgramSecurityLevel.Safe, ProgramPermissionAnalyzer.Analyze(ulong.MaxValue, false));
        Assert.Equal(ProgramSecurityLevel.Safe, ProgramPermissionAnalyzer.Analyze(0x8000000000000000, false));
    }

    [Theory]
    [InlineData(0x0000000000000000, ProgramSecurityLevel.Safe)]
    [InlineData(0x0000000000001234, ProgramSecurityLevel.Safe)]
    [InlineData(0x8000000000000000, ProgramSecurityLevel.Unsafe)]
    [InlineData(0x8000000000000080, ProgramSecurityLevel.Unsafe)]
    [InlineData(ulong.MaxValue, ProgramSecurityLevel.Dangerous)]
    public void Analyze_ClassifiesFileSystemPermissions(ulong permissions, ProgramSecurityLevel expected)
    {
        Assert.Equal(expected, ProgramPermissionAnalyzer.Analyze(permissions));
    }
}
