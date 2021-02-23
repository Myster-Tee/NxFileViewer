using System;
using System.IO;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Services;
using LibHac;
using LibHac.Fs;
using LibHac.FsSystem;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.FileLoading
{
    public class FileItemLoader : IFileItemLoader
    {
        private readonly IKeySetProviderService _keySetProviderService;
        private readonly IServiceProvider _serviceProvider;

        public FileItemLoader(IKeySetProviderService keySetProviderService, IServiceProvider serviceProvider)
        {
            _keySetProviderService = keySetProviderService ?? throw new ArgumentNullException(nameof(keySetProviderService));
            _serviceProvider = serviceProvider;
        }

        public NspItem LoadNsp(string nspFilePath)
        {
            var keySet = _keySetProviderService.GetKeySet();

            var localFile = new LocalFile(nspFilePath, OpenMode.Read);

            var fileStorage = new FileStorage(localFile);
            var nspPartition = new PartitionFileSystem(fileStorage);

            var nspItem = new NspItem(nspPartition, Path.GetFileName(nspFilePath), localFile, keySet, _serviceProvider.GetRequiredService<IChildItemsBuilder>());

            return nspItem;
        }

        public XciItem LoadXci(string xciFilePath)
        {
            var keySet = _keySetProviderService.GetKeySet();

            var localFile = new LocalFile(xciFilePath, OpenMode.Read);

            var fileStorage = new FileStorage(localFile);
            var xci = new Xci(keySet, fileStorage);

            var xciItem = new XciItem(xci, Path.GetFileName(xciFilePath), localFile, keySet, _serviceProvider.GetRequiredService<IChildItemsBuilder>());

            return xciItem;
        }

    }
}
