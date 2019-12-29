using System;
using LibHac;
using LibHac.Fs;

namespace Emignatik.NxFileViewer.Utils
{
    public static class LibHacExt
    {

        public static IFile OpenFile(this IFileSystem fileSystem, string path, OpenMode mode)
        {
            var result = fileSystem.OpenFile(out var file, path, OpenMode.Read);
            ThrowOnErrorResult(ref result);
            return file;
        }

        public static long GetSize(this IFile file)
        {
            var result = file.GetSize(out var size);
            ThrowOnErrorResult(ref result);
            return size;
        }

        public static void ThrowOnErrorResult(ref Result result)
        {
            if (result.IsFailure())
                throw new Exception(result.ToStringWithName()); //TODO: analyser si c'est la bonne méthode
        }
    }
}
