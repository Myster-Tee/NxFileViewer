using System;
using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using LibHac.Common;
using LibHac.Common.Keys;
using LibHac.Fs;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;
using LibHac.Ns;
using LibHac.Tools.Fs;
using LibHac.Tools.FsSystem;
using LibHac.Tools.FsSystem.NcaUtils;
using LibHac.Tools.Ncm;

namespace Emignatik.NxFileViewer.Utils;

public static class LibHacHelperExtension
{

    public static IEnumerable<DirectoryEntryEx> FindCnmtEntries(this IFileSystem fileSystem)
    {
        foreach (var fileEntry in fileSystem.EnumerateEntries().Where(e => e.Type == DirectoryEntryType.File))
        {
            var fileName = fileEntry.Name;
            if (fileName.EndsWith("cnmt.nca", StringComparison.OrdinalIgnoreCase))
                yield return fileEntry;
        }
    }

    public static Nca? LoadNca(this IFileSystem fileSystem, string ncaId, KeySet keySet)
    {
        var partitionFileEntry = fileSystem.EnumerateEntries()
            .Where(e => e.Type == DirectoryEntryType.File)
            .FirstOrDefault(entry => entry.Name.StartsWith(ncaId + ".", StringComparison.OrdinalIgnoreCase));

        if (partitionFileEntry == null)
            return null;

        var ncaFile = fileSystem.LoadFile(partitionFileEntry);

        return new Nca(keySet, new FileStorage(ncaFile));
    }


    public static IEnumerable<Cnmt> LoadCnmts(this IFileSystem fileSystem, KeySet keySet)
    {
        foreach (var cnmtFileEntry in fileSystem.FindCnmtEntries())
        {
            var ncaFile = fileSystem.LoadFile(cnmtFileEntry);

            var nca = new Nca(keySet, new FileStorage(ncaFile));

            if (nca.Header.ContentType != NcaContentType.Meta)// NOTE: this test is normally not necessary because CNMT are always of type NcaContentType.Meta
                continue;


            var cnmtEntries = nca.FindEntriesAmongSections("*.cnmt");

            foreach (var (sectionFileSystem, cnmtEntry) in cnmtEntries)
            {
                var cnmtFile = sectionFileSystem.LoadFile(cnmtEntry);

                var cnmt = new Cnmt(cnmtFile.AsStream());

                yield return cnmt;

            }
        }
    }

    /// <summary>
    /// Loads control.nacp file contained in the specified NCA.
    /// </summary>
    /// <param name="fileSystem"></param>
    /// <param name="ncaId">The ID of the NCA of ContentType <see cref="ContentType.Control"/></param>
    /// <param name="keySet"></param>
    /// <returns></returns>
    public static ApplicationControlProperty? LoadNacp(this IFileSystem fileSystem, string ncaId, KeySet keySet)
    {
        var nca = fileSystem.LoadNca(ncaId, keySet);
        if (nca == null)
            return null;

        var foundEntry = nca.FindEntriesAmongSections("control.nacp").FirstOrDefault();
        if (foundEntry == null)
            return null;

        var (sectionFileSystem, nacpEntry) = foundEntry;

        var nacpFile = sectionFileSystem.LoadFile(nacpEntry);

        var blitStruct = new BlitStruct<ApplicationControlProperty>(1);
        nacpFile.Read(out _, 0, blitStruct.ByteSpan).ThrowIfFailure();

        return blitStruct.Value;
    }

    public static IEnumerable<FoundEntry> FindEntriesAmongSections(this Nca nca, string searchPattern, SearchOptions searchOptions = SearchOptions.RecurseSubdirectories | SearchOptions.CaseInsensitive)
    {
        for (var sectionIndex = 0; sectionIndex < NcaItem.MaxSections; sectionIndex++)
        {
            if (!nca.Header.IsSectionEnabled(sectionIndex))
                continue;

            var sectionFileSystem = nca.OpenFileSystem(sectionIndex, IntegrityCheckLevel.ErrorOnInvalid);

            var entries = sectionFileSystem.EnumerateEntries("/", searchPattern, searchOptions);
            foreach (var entry in entries)
            {
                yield return new FoundEntry(sectionFileSystem, entry);
            }
        }
    }


    public static IFile LoadFile(this IFileSystem fileSystem, DirectoryEntryEx directoryEntryEx, OpenMode openMode = OpenMode.Read)
    {
        using var uniqueRefFile = new UniqueRef<IFile>();
        fileSystem.OpenFile(ref uniqueRefFile.Ref, directoryEntryEx.FullPath.ToU8Span(), openMode).ThrowIfFailure();
        return uniqueRefFile.Release();
    }


    public static int GetPatchNumber(this TitleVersion titleVersion)
    {
        return titleVersion.Minor;
    }

    public static PartitionFileSystem LoadPartition(this IStorage storage)
    {
        var pfs = new UniqueRef<PartitionFileSystem>(new PartitionFileSystem());
        pfs.Get.Initialize(storage).ThrowIfFailure();
        return pfs.Get;
    }
}

public class FoundEntry
{
    public FoundEntry(IFileSystem sectionFileSystem, DirectoryEntryEx entry)
    {
        SectionFileSystem = sectionFileSystem;
        Entry = entry;
    }

    public IFileSystem SectionFileSystem { get; }

    public DirectoryEntryEx Entry { get; }


    public void Deconstruct(out IFileSystem sectionFileSystem, out DirectoryEntryEx entry)
    {
        sectionFileSystem = SectionFileSystem;
        entry = Entry;
    }
}