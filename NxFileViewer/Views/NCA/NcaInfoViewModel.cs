using System;
using System.Linq;
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
                var definedSectionIndexes = _pfsNcaFile.DefinedSections.Select(sectionHeader => $"{sectionHeader.Index}: {sectionHeader.FormatType}");
                return string.Join(Environment.NewLine, definedSectionIndexes);
            }
        }

        public string SdkVersion => _pfsNcaFile.SdkVersion;

        //TODO à finir!
        public string CryptoType1 => ""; //_pfsNcaFile.Header.CryptoType.ToString();

        //TODO à finir!
        public string CryptoType2 => ""; //_pfsNcaFile.RawStruct.CryptoType2.ToString();
    }
}