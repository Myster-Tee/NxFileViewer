namespace Emignatik.NxFileViewer.FileLoading.QuickFileInfoLoading;

public interface IPackageInfoLoader
{
    PackageInfo GetPackageInfo(string filePath);
}