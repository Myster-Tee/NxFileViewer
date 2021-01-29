using System;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Utils;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac;
using LibHac.Fs;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public class NacpItem : DirectoryEntryItem
    {
        public const string NacpFileName = "control.nacp";

        public NacpItem(Nacp nacp, SectionItem parentSectionItem, DirectoryEntry directoryEntry, string name, string path, IChildItemsBuilder childItemsBuilder) : base(parentSectionItem, directoryEntry, name, path, childItemsBuilder)
        {
            Nacp = nacp ?? throw new ArgumentNullException(nameof(nacp));
        }

        public Nacp Nacp { get; }

        [PropertiesView]
        public string UnderlyingType => nameof(Nacp);

        [PropertiesView]
        public string DisplayVersion => Nacp.DisplayVersion;

        [PropertiesView]
        public string AddOnContentBaseId => Nacp.AddOnContentBaseId.ToStrId();

        [PropertiesView]
        public string Isbn => Nacp.Isbn;

        [PropertiesView]
        public string ApplicationErrorCodeCategory => Nacp.ApplicationErrorCodeCategory;

        [PropertiesView]
        public string BcatPassphrase => Nacp.BcatPassphrase;

        [PropertiesView]
        public uint ParentalControlFlag => Nacp.ParentalControlFlag;

    }
}