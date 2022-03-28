using System;
using System.Collections.Generic;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Fs.Fsa;
using LibHac.Tools.Fs;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

/// <summary>
/// Can be either a file or a directory from a <see cref="IFileSystem"/> LibHac model
/// </summary>
public class DirectoryEntryItem : ItemBase
{
    /// <summary>
    /// Constructor when entry is direct child of a <see cref="FsSectionItem"/>
    /// </summary>
    /// <param name="parentFsItem"></param>
    /// <param name="directoryEntry"></param>
    public DirectoryEntryItem(FsSectionItem parentFsItem, DirectoryEntryEx directoryEntry)
        : this(parentFsItem, parentFsItem, directoryEntry)
    {
        ParentFsSectionItem = parentFsItem ?? throw new ArgumentNullException(nameof(parentFsItem));
    }

    /// <summary>
    /// Constructor when entry is direct child of another <see cref="DirectoryEntryItem"/>
    /// </summary>
    /// <param name="containerFsSectionItem"></param>
    /// <param name="directoryEntry"></param>
    /// <param name="parentItem"></param>
    public DirectoryEntryItem(FsSectionItem containerFsSectionItem, DirectoryEntryEx directoryEntry, DirectoryEntryItem parentItem)
        : this(parentItem, containerFsSectionItem, directoryEntry)
    {
        ParentDirectoryEntryItem = parentItem ?? throw new ArgumentNullException(nameof(parentItem));
    }

    private DirectoryEntryItem(ItemBase parentItem, FsSectionItem containerFsSectionItem, DirectoryEntryEx directoryEntry) : base(parentItem)
    {
        DirectoryEntry = directoryEntry;
        ContainerFsSectionItem = containerFsSectionItem ?? throw new ArgumentNullException(nameof(containerFsSectionItem));
        Size = directoryEntry.Size;
    }


    /// <summary>
    /// Get the parent <see cref="DirectoryEntryItem"/> or null (<see cref="ParentFsSectionItem"/>).
    /// 
    /// A <see cref="DirectoryEntryItem"/> can either be a child of another <see cref="DirectoryEntryItem"/>
    /// or a child of a <see cref="FsSectionItem"/>
    /// </summary>
    public DirectoryEntryItem? ParentDirectoryEntryItem { get; }

    /// <summary>
    /// Get the parent <see cref="FsSectionItem"/> or null (<see cref="ParentDirectoryEntryItem"/>).
    /// 
    /// A <see cref="DirectoryEntryItem"/> can either be a child of another <see cref="DirectoryEntryItem"/>
    /// or a child of a <see cref="FsSectionItem"/>
    /// </summary>
    public FsSectionItem? ParentFsSectionItem { get; }

    /// <summary>
    /// Get the section which contains this <see cref="DirectoryEntryItem"/> in its descendants
    /// </summary>
    public FsSectionItem ContainerFsSectionItem { get; }

    /// <summary>
    /// Get the child directory entries (can be either a file or a directory)
    /// </summary>
    public sealed override List<DirectoryEntryItem> ChildItems { get; } = new();

    /// <summary>
    /// Get the <see cref="DirectoryEntry"/> metadata
    /// </summary>
    public DirectoryEntryEx DirectoryEntry { get; }

    public sealed override string LibHacTypeName => DirectoryEntry.GetType().Name;

    public override string? Format => null;

    public override string Name => DirectoryEntry.Name;

    public long Size { get; }

    public DirectoryEntryType DirectoryEntryType => DirectoryEntry.Type;

    public string Path => DirectoryEntry.FullPath;

    public override string DisplayName => Name;

    /// <summary>
    /// Should be called only for entries of type <see cref="LibHac.Fs.DirectoryEntryType.File"/>
    /// </summary>
    /// <returns></returns>
    public IFile GetFile()
    {
        using var uniqueRefFile = new UniqueRef<IFile>();
        this.ContainerFsSectionItem.FileSystem!.OpenFile(ref uniqueRefFile.Ref(), this.Path.ToU8Span(), OpenMode.Read).ThrowIfFailure();
        return uniqueRefFile.Release();
    }

    public override string ToString()
    {
        return $"{DisplayName} ({DirectoryEntry.Type})";
    }
}