using System;
using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.Services.Security;
using LibHac.Common;
using LibHac.Tools.Fs;
using LibHac.Tools.Npdm;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

public class NpdmItem : DirectoryEntryItem
{
    public const string NpdmFileName = "main.npdm";

    public NpdmItem(NpdmBinary npdm, SectionItem parentItem, DirectoryEntryEx directoryEntry)
        : base(parentItem, directoryEntry)
    {
        Npdm = npdm ?? throw new ArgumentNullException(nameof(npdm));
        FileSystemPermissions = npdm.AciD.FsAccess.PermissionsBitmask;

        AcidSignatureValidity = npdm.AciD.SignatureValidity;
        Services = npdm.AciD.ServiceAccess?.Services?
            .Where(service => !string.IsNullOrWhiteSpace(service.Item1))
            .Select(service => service.Item1)
            .OrderBy(service => service, StringComparer.Ordinal)
            .ToArray() ?? [];
        SecurityLevel = ProgramPermissionAnalyzer.Analyze(FileSystemPermissions, ProgramPermissionAnalyzer.HasFileSystemServices(Services));
    }

    public NpdmBinary Npdm { get; }

    public ulong FileSystemPermissions { get; }

    public ProgramSecurityLevel SecurityLevel { get; }

    public Validity AcidSignatureValidity { get; }

    public IReadOnlyList<string> Services { get; }

    public override string Format => "NPDM";
}
