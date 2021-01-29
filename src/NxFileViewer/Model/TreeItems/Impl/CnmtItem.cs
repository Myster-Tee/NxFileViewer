using System;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Utils;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac;
using LibHac.Fs;
using LibHac.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public class CnmtItem : DirectoryEntryItem
    {
        public CnmtItem(Cnmt cnmt, SectionItem parentSectionItem, DirectoryEntry directoryEntry, string name, string path, IChildItemsBuilder childItemsBuilder)
            : base(parentSectionItem, directoryEntry, name, path, childItemsBuilder)
        {
            Cnmt = cnmt ?? throw new ArgumentNullException(nameof(cnmt));
            PatchLevel = GetPatchLevel(Cnmt.TitleVersion);
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
        public string? TitleVersion => Cnmt.TitleVersion?.Version.ToString();

        [PropertiesView]
        public int? PatchLevel { get; }

        [PropertiesView]
        public string? MinimumApplicationVersion => Cnmt.MinimumApplicationVersion?.ToString();

        [PropertiesView]
        public string? MinimumSystemVersion => Cnmt.MinimumSystemVersion?.ToString();

        private static int? GetPatchLevel(TitleVersion? titleVersion)
        {
            var version = titleVersion?.Version;
            const int n = 65536;
            if (version != null && version % n == 0)
                return (int)version / n;
            return null;
        }
    }
}