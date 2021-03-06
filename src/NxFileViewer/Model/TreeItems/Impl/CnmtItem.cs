﻿using System;
using Emignatik.NxFileViewer.Utils;
using LibHac;
using LibHac.Fs;
using LibHac.FsSystem.NcaUtils;
using LibHac.Ncm;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public class CnmtItem : DirectoryEntryItem
    {
        public CnmtItem(Cnmt cnmt, SectionItem parentSectionItem, DirectoryEntry directoryEntry, string name, string path)
            : base(parentSectionItem, directoryEntry, name, path)
        {
            Cnmt = cnmt ?? throw new ArgumentNullException(nameof(cnmt));
            ParentItem = parentSectionItem;
            PatchLevel = GetPatchLevel(Cnmt.TitleVersion);
            TitleId = Cnmt.TitleId.ToStrId();
            ApplicationTitleId = Cnmt.ApplicationTitleId.ToStrId();
            PatchTitleId = Cnmt.PatchTitleId.ToStrId();
            TitleVersion = Cnmt.TitleVersion?.Version.ToString();
        }

        public sealed override SectionItem ParentItem { get; }

        public Cnmt Cnmt { get; }

        public override string LibHacUnderlyingTypeName => nameof(Cnmt);

        public ContentMetaType ContentType => Cnmt.Type;

        public string TitleId { get; }

        public string ApplicationTitleId { get; }

        public string PatchTitleId { get; }

        public string? TitleVersion { get; }

        public int? PatchLevel { get; }

        public TitleVersion? MinimumApplicationVersion => Cnmt.MinimumApplicationVersion;

        public TitleVersion? MinimumSystemVersion => Cnmt.MinimumSystemVersion;

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