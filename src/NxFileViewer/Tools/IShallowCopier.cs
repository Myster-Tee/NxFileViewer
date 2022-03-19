namespace Emignatik.NxFileViewer.Tools;

public interface IShallowCopier
{
    /// <summary>
    /// Recopy property values from source object to destination object.
    ///
    /// Only value types and string are copied deeply
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    void Copy<T>(T? source, T? dest) where T : class;
}