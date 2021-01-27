using System;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Utils;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac;
using LibHac.Fs;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public class CnmtItem : DirectoryEntryItem
    {
        public CnmtItem(Cnmt cnmt, SectionItem parentSectionItem, DirectoryEntry directoryEntry, string name, string path, IChildItemsBuilder childItemsBuilder) 
            : base(parentSectionItem, directoryEntry, name, path, childItemsBuilder)
        {
            Cnmt = cnmt ?? throw new ArgumentNullException(nameof(cnmt));
        }

        public Cnmt Cnmt { get; }

        [PropertiesView]
        public string UnderlyingType => nameof(Cnmt);

        [PropertiesView]
        public string ContentType => Cnmt.Type.ToString();

        [PropertiesView]
        public string TitleId => Cnmt.TitleId.ToStrId();

        [PropertiesView]
        public string ApplicationTitleId => Cnmt.ApplicationTitleId.ToStrId();

        [PropertiesView]
        public string PatchTitleId => Cnmt.PatchTitleId.ToStrId();

        [PropertiesView]
        public string? TitleVersion => Cnmt.TitleVersion?.ToString();

        [PropertiesView]
        public string? MinimumApplicationVersion => Cnmt.MinimumApplicationVersion?.ToString();

        [PropertiesView]
        public string? MinimumSystemVersion => Cnmt.MinimumSystemVersion?.ToString();
    }
}