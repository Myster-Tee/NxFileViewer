using Emignatik.NxFileViewer.Model.Overview;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;

namespace Emignatik.NxFileViewer.FileLoading;

public interface IFileOverviewLoader
{
    FileOverview Load(XciItem xciItem);

    FileOverview Load(NspItem nspItem);
}