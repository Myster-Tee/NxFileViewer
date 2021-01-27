using System;
using System.IO;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Services;
using LibHac;
using LibHac.Fs;
using LibHac.FsSystem;

namespace Emignatik.NxFileViewer.FileLoading
{
    public class FileItemLoader : IFileItemLoader
    {
        private readonly IKeySetProviderService _keySetProviderService;
        private readonly IChildItemsBuilder _childItemsBuilder;

        public FileItemLoader(IKeySetProviderService keySetProviderService, IChildItemsBuilder childItemsBuilder)
        {
            _keySetProviderService = keySetProviderService ?? throw new ArgumentNullException(nameof(keySetProviderService));
            _childItemsBuilder = childItemsBuilder ?? throw new ArgumentNullException(nameof(childItemsBuilder));
        }

        public NspItem LoadNsp(string nspFilePath)
        {
            var keySet = _keySetProviderService.GetKeySet();

            var localFile = new LocalFile(nspFilePath, OpenMode.Read);

            var fileStorage = new FileStorage(localFile);
            var nspPartition = new PartitionFileSystem(fileStorage);

            var nspItem = new NspItem(nspPartition, Path.GetFileName(nspFilePath), localFile, keySet, _childItemsBuilder);

            return nspItem;
        }

        public XciItem LoadXci(string xciFilePath)
        {
            var keySet = _keySetProviderService.GetKeySet();

            var localFile = new LocalFile(xciFilePath, OpenMode.Read);

            var fileStorage = new FileStorage(localFile);
            var xci = new Xci(keySet, fileStorage);

            var xciItem = new XciItem(xci, Path.GetFileName(xciFilePath), localFile, keySet, _childItemsBuilder);

            return xciItem;
        }

    }
}
