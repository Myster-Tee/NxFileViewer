﻿using System;
using LibHac;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Loader;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public class MainItem : DirectoryEntryItem
    {

        public MainItem(NsoHeader nsoHeader, SectionItem parentItem, DirectoryEntry directoryEntry, string name, string path)
            : base(parentItem, directoryEntry, name, path)
        {
            ParentItem = parentItem ?? throw new ArgumentNullException(nameof(parentItem));
            NsoHeader = nsoHeader;
        }

        public override SectionItem ParentItem { get; }

        public NsoHeader NsoHeader { get; }

        public override string LibHacUnderlyingTypeName => nameof(Nso);

        public Buffer32 ModuleId => NsoHeader.ModuleId;

    }
}
