using System;
using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Utils;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac.Fs;
using LibHac.Fs.Fsa;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    /// <summary>
    /// Can be either a file or a directory from a <see cref="IFileSystem"/> LibHac model
    /// </summary>
    public class DirectoryEntryItem : ItemBase
    {
        private readonly IChildItemsBuilder _childItemsBuilder;
        private List<DirectoryEntryItem>? _subDirEntries;

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
        {
            _childItemsBuilder = childItemsBuilder ?? throw new ArgumentNullException(nameof(childItemsBuilder));
            DirectoryEntry = directoryEntry;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ContainerSectionItem = containerSectionItem ?? throw new ArgumentNullException(nameof(containerSectionItem));
            Path = path ?? throw new ArgumentNullException(nameof(path));
            ParentItem = parentItem ?? throw new ArgumentNullException(nameof(parentItem));
            SizeStr = directoryEntry.Size.ToFileSize();
        }

        public sealed override string ObjectType => nameof(DirectoryEntry);

        [PropertiesView]
        public string DirectoryEntryType => DirectoryEntry.Type.ToString();

        [PropertiesView]
        public string Name { get; }

        [PropertiesView("Size")]
        public string SizeStr { get; }

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
        /// Get the <see cref="DirectoryEntry"/> metadata
        /// </summary>
        public DirectoryEntry DirectoryEntry { get; }

        protected override IEnumerable<IItem> LoadChildItems()
        {
            return GetChildDirectoryEntryItems();
        }

        private List<DirectoryEntryItem> GetChildDirectoryEntryItems()
        {
            return _subDirEntries ??= _childItemsBuilder.Build(this).ToList();
        }

        public override string ToString()
        {
            return $"{DisplayName} ({DirectoryEntry.Type})";
        }
    }
}