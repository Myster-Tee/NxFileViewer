using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.Models.Overview;
using Emignatik.NxFileViewer.Models.TreeItems;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Models;

public sealed class OverviewTraversalTest
{
    [Fact]
    public void EstimatedSizeDoesNotRescanContentWhenDisplayedAgain()
    {
        var root = new CountingItem("root");
        _ = new CountingItem("leaf", root);
        var overview = new FileOverview(root) { FileSize = 20 };
        Assert.Equal(20, overview.EstimatedUncompressedSize);
        Assert.Equal(20, overview.EstimatedUncompressedSize);
        Assert.Equal(1, root.ChildReads);
        overview.FileSize = 30;
        Assert.Equal(30, overview.EstimatedUncompressedSize);
        Assert.Equal(1, root.ChildReads);
    }

    [Fact]
    public void TraversalKeepsBreadthFirstOrderAndIncludeRootBehavior()
    {
        var root = new CountingItem("root");
        var a = new CountingItem("a", root);
        var b = new CountingItem("b", root);
        _ = new CountingItem("a1", a);
        _ = new CountingItem("b1", b);
        Assert.Equal(new[] { "root", "a", "b", "a1", "b1" },
            root.FindChildrenOfType<IItem>(true).Select(item => item.Name));
        Assert.Equal(new[] { "a", "b", "a1", "b1" },
            root.FindChildrenOfType<IItem>(false).Select(item => item.Name));
    }

    [Fact]
    public void WideTreeVisitsEveryNodeOnce()
    {
        var root = new CountingItem("root");
        var children = Enumerable.Range(0, 20000).Select(i => new CountingItem(i.ToString(), root)).ToArray();
        Assert.Equal(20001, root.FindChildrenOfType<IItem>(true).Count());
        Assert.Equal(1, root.ChildReads);
        Assert.All(children, child => Assert.Equal(1, child.ChildReads));
    }

    private sealed class CountingItem : ItemBase, IItem
    {
        public CountingItem(string name, CountingItem? parent = null) : base(parent) { Name = name; }
        public int ChildReads { get; private set; }
        public new IReadOnlyList<IItem> ChildItems { get { ChildReads++; return base.ChildItems; } }
        public override string Name { get; }
        public override string DisplayName => Name;
        public override string Format => "test";
        public override string LibHacTypeName => "test";
    }
}