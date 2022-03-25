namespace Emignatik.NxFileViewer.FileLoading;

public interface IPackageTypeAnalyzer
{
    /// <summary>
    /// Try to detect the real type of the specified file
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    PackageType GetType(string filePath);
}


public enum PackageType
{
    UNKNOWN,
    XCI,
    NSP
}