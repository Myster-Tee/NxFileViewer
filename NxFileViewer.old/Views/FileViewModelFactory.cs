using Emignatik.NxFileViewer.NSP.Models;
using Emignatik.NxFileViewer.Views.NCA;
using Emignatik.NxFileViewer.Views.NSP;

namespace Emignatik.NxFileViewer.Views
{
    public class FileViewModelFactory
    {

        public FileViewModelBase Create(PfsFile pfsFile)
        {
            if (pfsFile is PfsNcaFile pfsNcaFile)
                return new NcaInfoViewModel(pfsNcaFile);
            else
                return new PfsFileViewModel(pfsFile);

        }

    }
}