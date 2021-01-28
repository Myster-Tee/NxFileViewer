using System;
using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac.Fs.Fsa;
using LibHac.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public class SectionItem : ItemBase
    {
        private readonly NcaFsHeader _ncaFsHeader;
        private readonly IChildItemsBuilder _childItemsBuilder;
        private List<DirectoryEntryItem>? _dirEntries;

        public SectionItem(int sectionIndex, NcaFsHeader ncaFsHeader, IFileSystem fileSystem, NcaItem parentNcaItem,
            IChildItemsBuilder childItemsBuilder)
        {
            _ncaFsHeader = ncaFsHeader;
            _childItemsBuilder = childItemsBuilder ?? throw new ArgumentNullException(nameof(childItemsBuilder));
            SectionIndex = sectionIndex;
            FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            ParentNcaItem = parentNcaItem ?? throw new ArgumentNullException(nameof(parentNcaItem));
        }

        public override string ObjectType => "Section";

        public override string DisplayName => $"Section {SectionIndex}";

        public NcaItem ParentNcaItem { get; }

        public override IItem ParentItem => ParentNcaItem;

        public IReadOnlyList<DirectoryEntryItem> ChildDirectoryEntryItems => GetChildDirectoryEntryItems();

        [PropertiesView]
        public int SectionIndex { get; }

        [PropertiesView] 
        public string EncryptionType => _ncaFsHeader.EncryptionType.ToString();

        [PropertiesView] 
        public string FormatType => _ncaFsHeader.FormatType.ToString();    
        
        [PropertiesView] 
        public string HashType => _ncaFsHeader.HashType.ToString();   
        
        [PropertiesView] 
        public string Version => _ncaFsHeader.Version.ToString();

        public IFileSystem FileSystem { get; }

        protected override IEnumerable<IItem> LoadChildItems()
        {
            return GetChildDirectoryEntryItems();
        }

        private List<DirectoryEntryItem> GetChildDirectoryEntryItems()
        {
            return _dirEntries ??= _childItemsBuilder.Build(this).ToList();
        }
    }
}