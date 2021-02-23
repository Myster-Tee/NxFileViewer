using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.FileLoading;
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
        private IReadOnlyList<DirectoryEntryItem>? _subDirEntries;

        public DirectoryEntryItem(SectionItem containerSectionItem, DirectoryEntry directoryEntry, string name, string path, DirectoryEntryItem parentDirectoryEntryItem, IChildItemsBuilder childItemsBuilder)
            : this(containerSectionItem, directoryEntry, name, path, (IItem)parentDirectoryEntryItem, childItemsBuilder)
        {
            ParentDirectoryEntryItem = parentDirectoryEntryItem;
        }

        public DirectoryEntryItem(SectionItem parentSectionItem, DirectoryEntry directoryEntry, string name, string path, IChildItemsBuilder childItemsBuilder)
            : this(parentSectionItem, directoryEntry, name, path, parentSectionItem, childItemsBuilder)
        {
            ParentSectionItem = parentSectionItem;
        }

        private DirectoryEntryItem(SectionItem containerSectionItem, DirectoryEntry directoryEntry, string name, string path, IItem parentItem, IChildItemsBuilder childItemsBuilder)
            : base(childItemsBuilder)
        {
            DirectoryEntry = directoryEntry;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ContainerSectionItem = containerSectionItem ?? throw new ArgumentNullException(nameof(containerSectionItem));
            Path = path ?? throw new ArgumentNullException(nameof(path));
            ParentItem = parentItem ?? throw new ArgumentNullException(nameof(parentItem));
            Size = directoryEntry.Size;
        }

        /// <summary>
        /// Get the <see cref="DirectoryEntry"/> metadata
        /// </summary>
        public DirectoryEntry DirectoryEntry { get; }

        public sealed override string LibHacTypeName => nameof(DirectoryEntry);

        public override string? LibHacUnderlyingTypeName => null;

        public override string Name { get; }

        public long Size { get; }

        public DirectoryEntryType DirectoryEntryType => DirectoryEntry.Type;

        public string Path { get; }

        public override string DisplayName => Name;

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

        public override IItem ParentItem { get; }

        /// <summary>
        /// Get the child directory entries (can be either a file or a directory)
        /// </summary>
        public IReadOnlyList<DirectoryEntryItem> ChildDirectoryEntryItems => GetChildDirectoryEntryItems();

        /// <summary>
        /// Get the section which contains this <see cref="DirectoryEntryItem"/> in its descendants
        /// </summary>
        public SectionItem ContainerSectionItem { get; }

        /// <summary>
        /// Should be called only for entries of type <see cref="LibHac.Fs.DirectoryEntryType.File"/>
        /// </summary>
        /// <returns></returns>
        public IFile GetFile()
        {
            this.ContainerSectionItem.FileSystem.OpenFile(out var file, new U8Span(this.Path), OpenMode.Read).ThrowIfFailure();
            return file;
        }

        protected override IReadOnlyList<IItem> SafeLoadChildItemsInternal()
        {
            return GetChildDirectoryEntryItems();
        }

        private IReadOnlyList<DirectoryEntryItem> GetChildDirectoryEntryItems()
        {
            return _subDirEntries ??= ChildItemsBuilder.Build(this);
        }

        public override string ToString()
        {
            return $"{DisplayName} ({DirectoryEntry.Type})";
        }
    }
}