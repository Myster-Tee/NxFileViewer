using System;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Utils;
using LibHac;
using LibHac.Fs;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public class NacpItem : DirectoryEntryItem
    {
        public const string NacpFileName = "control.nacp";

        public NacpItem(Nacp nacp, SectionItem parentSectionItem, DirectoryEntry directoryEntry, string name, string path, IChildItemsBuilder childItemsBuilder)
            : base(parentSectionItem, directoryEntry, name, path, childItemsBuilder)
        {
            Nacp = nacp ?? throw new ArgumentNullException(nameof(nacp));
            AddOnContentBaseId = Nacp.AddOnContentBaseId.ToStrId();
        }

        public Nacp Nacp { get; }

        public override string LibHacUnderlyingTypeName => nameof(Nacp);

        public string DisplayVersion => Nacp.DisplayVersion;

        public string AddOnContentBaseId { get; }

        public string Isbn => Nacp.Isbn;

        public string ApplicationErrorCodeCategory => Nacp.ApplicationErrorCodeCategory;

        public string BcatPassphrase => Nacp.BcatPassphrase;

        public uint ParentalControlFlag => Nacp.ParentalControlFlag;
    }
}