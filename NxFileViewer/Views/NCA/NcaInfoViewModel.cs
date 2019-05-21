using System;
using Emignatik.NxFileViewer.NSP.Models;
using Emignatik.NxFileViewer.Utils;

namespace Emignatik.NxFileViewer.Views.NCA
{
    public class NcaInfoViewModel : FileViewModelBase
    {
        private readonly PfsNcaFile _pfsNcaFile;

        public NcaInfoViewModel(PfsNcaFile pfsNcaFile)
        {
            _pfsNcaFile = pfsNcaFile ?? throw new ArgumentNullException(nameof(pfsNcaFile));
        }

        public string TitleId => _pfsNcaFile.TitleId.ToHex();

        public string Type => _pfsNcaFile.ContentType.ToString();

        public string DefinedSections
        {
            get
            {
                //TODO à finir!

                return "";
                //var definedSectionIndexes = _pfsNcaFile.Header..DefinedSections.Select(sectionHeader => (int)sectionHeader.SectionIndex);
                //return string.Join(",", definedSectionIndexes);
            }
        }

        public string SdkVersion => _pfsNcaFile.SdkVersion;

        //TODO à finir!
        public string CryptoType1 => ""; //_pfsNcaFile.Header.CryptoType.ToString();

        //TODO à finir!
        public string CryptoType2 => ""; //_pfsNcaFile.RawStruct.CryptoType2.ToString();
    }
}