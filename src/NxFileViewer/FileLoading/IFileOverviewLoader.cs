using Emignatik.NxFileViewer.Models.Overview;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;

namespace Emignatik.NxFileViewer.FileLoading;

public interface IFileOverviewLoader
{
    FileOverview Load(XciItem xciItem);

    FileOverview Load(NspItem nspItem);
}