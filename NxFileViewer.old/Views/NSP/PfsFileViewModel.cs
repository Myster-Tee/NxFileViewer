using System;
using Emignatik.NxFileViewer.NSP.Models;
using Emignatik.NxFileViewer.Utils;

namespace Emignatik.NxFileViewer.Views.NSP
{
    public class PfsFileViewModel : FileViewModelBase
    {
        private readonly PfsFile _pfsFile;

        public PfsFileViewModel(PfsFile file)
        {
            _pfsFile = file ?? throw new ArgumentNullException(nameof(file));
            FileName = $"{_pfsFile.Name}  ({_pfsFile.Size.ToFileSize()})";
        }
    }
}
