using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;

namespace Emignatik.NxFileViewer.Model.Overview
{
    /// <summary>
    /// Aggregates an NacpItem with its corresponding titles <see cref="Titles"/>
    /// </summary>
    public class NacpContainer
    {

        public NacpContainer(NacpItem nacpItem)
        {
            NacpItem = nacpItem ?? throw new ArgumentNullException(nameof(nacpItem));
        }

        public NacpItem NacpItem { get; }

        public List<TitleInfo> Titles { get; } = new();

    }
}