using System;
using System.Collections.Generic;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Fs.Fsa;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    /// <summary>
    /// Can be either a file or a directory from a <see cref="IFileSystem"/> LibHac model
    /// </summary>
    public class DirectoryEntryItem : ItemBase
    {
        /// <summary>
        /// Constructor when entry is direct child of a <see cref="SectionItem"/>
        /// </summary>
        /// <param name="parentItem"></param>
        /// <param name="directoryEntry"></param>
        public DirectoryEntryItem(SectionItem parentItem, DirectoryEntryEx directoryEntry)
            : this(parentItem, parentItem, directoryEntry)
        {
            ParentSectionItem = parentItem;
        }

        /// <summary>
        /// Constructor when entry is direct child of another <see cref="DirectoryEntryItem"/>
        /// </summary>
        /// <param name="containerSectionItem"></param>
        /// <param name="directoryEntry"></param>
        /// <param name="parentItem"></param>
        public DirectoryEntryItem(SectionItem containerSectionItem, DirectoryEntryEx directoryEntry, DirectoryEntryItem parentItem)
            : this(parentItem, containerSectionItem, directoryEntry)
        {
            ParentDirectoryEntryItem = parentItem;
        }

        private DirectoryEntryItem(IItem parentItem, SectionItem containerSectionItem, DirectoryEntryEx directoryEntry)
        {
            DirectoryEntry = directoryEntry;
            ContainerSectionItem = containerSectionItem ?? throw new ArgumentNullException(nameof(containerSectionItem));
            ParentItem = parentItem ?? throw new ArgumentNullException(nameof(parentItem));
            Size = directoryEntry.Size;
        }

        public override IItem ParentItem { get; }

        /// <summary>
        /// Get the parent <see cref="DirectoryEntryItem"/> or null (<see cref="ParentSectionItem"/>).
        /// 
        /// A <see cref="DirectoryEntryItem"/> can either be a child of another <see cref="DirectoryEntryItem"/>
        /// or a child of a <see cref="SectionItem"/>
        /// </summary>
        public DirectoryEntryItem? ParentDirectoryEntryItem { get; }

        /// <summary>
        /// Get the parent <see cref="SectionItem"/> or null (<see cref="ParentDirectoryEntryItem"/>).
        /// 
        /// A <see cref="DirectoryEntryItem"/> can either be a child of another <see cref="DirectoryEntryItem"/>
        /// or a child of a <see cref="SectionItem"/>
        /// </summary>
        public SectionItem? ParentSectionItem { get; }

        /// <summary>
        /// Get the section which contains this <see cref="DirectoryEntryItem"/> in its descendants
        /// </summary>
        public SectionItem ContainerSectionItem { get; }

        /// <summary>
        /// Get the child directory entries (can be either a file or a directory)
        /// </summary>
        public sealed override List<DirectoryEntryItem> ChildItems { get; } = new();

        /// <summary>
        /// Get the <see cref="DirectoryEntry"/> metadata
        /// </summary>
        public DirectoryEntryEx DirectoryEntry { get; }

        public sealed override string LibHacTypeName => nameof(DirectoryEntry);

        public override string? LibHacUnderlyingTypeName => null;

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
            this.ContainerSectionItem.FileSystem!.OpenFile(ref uniqueRefFile.Ref(), this.Path.ToU8Span(), OpenMode.Read).ThrowIfFailure();
            return uniqueRefFile.Release();
        }

        public override string ToString()
        {
            return $"{DisplayName} ({DirectoryEntry.Type})";
        }
    }
}