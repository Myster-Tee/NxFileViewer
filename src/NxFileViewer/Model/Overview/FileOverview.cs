using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.Model.TreeItems;

namespace Emignatik.NxFileViewer.Model.Overview
{
    public class FileOverview   
    {

        public FileOverview(IItem rootItem)
        {
            RootItem = rootItem ?? throw new ArgumentNullException(nameof(rootItem));
        }

        public IItem RootItem { get; }

        public List<CnmtContainer> CnmtContainers { get; } = new List<CnmtContainer>();

        public PackageType PackageType { get; internal set; } = PackageType.Unknown;

    }

    public enum PackageType
    {
        SuperXCI,
        SuperNSP,
        Normal,
        Unknown,
    }
}