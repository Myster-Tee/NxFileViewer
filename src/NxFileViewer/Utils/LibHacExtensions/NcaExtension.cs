using System.Collections.Generic;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using LibHac.Fs.Fsa;
using LibHac.Tools.Fs;
using LibHac.Tools.FsSystem;
using LibHac.Tools.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Utils.LibHacExtensions;

public static class NcaExtension
{

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