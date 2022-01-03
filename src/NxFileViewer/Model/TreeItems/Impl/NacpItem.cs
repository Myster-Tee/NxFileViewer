using System;
using Emignatik.NxFileViewer.Utils;
using LibHac.Fs;
using LibHac.Ns;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public class NacpItem : DirectoryEntryItem
    {
        public const string NacpFileName = "control.nacp";

        public NacpItem(ApplicationControlProperty nacp, SectionItem parentItem, DirectoryEntry directoryEntry, string name, string path)
            : base(parentItem, directoryEntry, name, path)
        {
            Nacp = nacp;
            ParentItem = parentItem ?? throw new ArgumentNullException(nameof(parentItem));
            AddOnContentBaseId = Nacp.AddOnContentBaseId.ToStrId();
        }

        public override SectionItem ParentItem { get; }

        public ApplicationControlProperty Nacp { get; }

        public override string LibHacUnderlyingTypeName => nameof(Nacp);

        public string DisplayVersion => Nacp.DisplayVersion.ToString();

        public string AddOnContentBaseId { get; }

        public string Isbn => Nacp.Isbn.ToString();

        public string ApplicationErrorCodeCategory => Nacp.ApplicationErrorCodeCategory.ToString();

        public string BcatPassphrase => Nacp.BcatPassphrase.ToString();

        public ParentalControlFlagValue ParentalControl => Nacp.ParentalControl;
    }
}