using LibHac.Fs;

namespace Emignatik.NxFileViewer.Utils
{
    public static class LibHacExt
    {

        public static IFile OpenFile(this IFileSystem fileSystem, string path, OpenMode mode)
        {
            fileSystem.OpenFile(out var file, path, OpenMode.Read).ThrowIfFailure();
            return file;
        }

        public static long GetSize(this IFile file)
        {
            file.GetSize(out var size).ThrowIfFailure();
            return size;
        }

    }
}
