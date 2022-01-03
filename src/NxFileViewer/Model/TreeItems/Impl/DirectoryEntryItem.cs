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
        /// <param name="name"></param>
        /// <param name="path"></param>
        public DirectoryEntryItem(SectionItem parentItem, DirectoryEntry directoryEntry, string name, string path)
            : this(parentItem, parentItem, directoryEntry, name, path)
        {
            ParentSectionItem = parentItem;
        }

        /// <summary>
        /// Constructor when entry is direct child of another <see cref="DirectoryEntryItem"/>
        /// </summary>
        /// <param name="containerSectionItem"></param>
        /// <param name="directoryEntry"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="parentItem"></param>
        public DirectoryEntryItem(SectionItem containerSectionItem, DirectoryEntry directoryEntry, string name, string path, DirectoryEntryItem parentItem)
            : this(parentItem, containerSectionItem, directoryEntry, name, path)
        {
            ParentDirectoryEntryItem = parentItem;
        }

        private DirectoryEntryItem(IItem parentItem, SectionItem containerSectionItem, DirectoryEntry directoryEntry, string name, string path)
        {
            DirectoryEntry = directoryEntry;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ContainerSectionItem = containerSectionItem ?? throw new ArgumentNullException(nameof(containerSectionItem));
            Path = path ?? throw new ArgumentNullException(nameof(path));
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
        public DirectoryEntry DirectoryEntry { get; }

        public sealed override string LibHacTypeName => nameof(DirectoryEntry);

        public override string? LibHacUnderlyingTypeName => null;

        public override string Name { get; }

        public long Size { get; }

        public DirectoryEntryType DirectoryEntryType => DirectoryEntry.Type;

        public string Path { get; }

        public override string DisplayName => Name;

        /// <summary>
        /// Should be called only for entries of type <see cref="LibHac.Fs.DirectoryEntryType.File"/>
        /// </summary>
        /// <returns></returns>
        public IFile GetFile()
        {
            var file = new UniqueRef<IFile>();
            this.ContainerSectionItem.FileSystem!.OpenFile(ref file, new U8Span(this.Path), OpenMode.Read).ThrowIfFailure();
            return file.Get;
        }

        public override string ToString()
        {
            return $"{DisplayName} ({DirectoryEntry.Type})";
        }
    }
}