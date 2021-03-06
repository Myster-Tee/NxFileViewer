﻿using System;
using System.Collections.Generic;
using LibHac;
using LibHac.Fs.Fsa;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public class XciItem : ItemBase
    {
        private readonly IFile _file;

        public XciItem(Xci xci, string name, IFile file, Keyset keySet)
        {
            Xci = xci ?? throw new ArgumentNullException(nameof(xci));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _file = file ?? throw new ArgumentNullException(nameof(file));
            KeySet = keySet ?? throw new ArgumentNullException(nameof(keySet));
        }

        public override IItem? ParentItem => null;

        public override List<XciPartitionItem> ChildItems { get; } = new();

        public Xci Xci { get; }

        public override string LibHacTypeName => nameof(Xci);

        public override string? LibHacUnderlyingTypeName => null;

        public override string Name { get; }

        public override string DisplayName => Name;

        public Keyset KeySet { get; }

        public override void Dispose()
        {
            _file.Dispose();
        }
    }
}