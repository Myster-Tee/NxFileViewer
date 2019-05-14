using System;
using System.Linq;
using Emignatik.NxFileViewer.NxFormats.NCA.Models;

namespace Emignatik.NxFileViewer.Views.NCA
{
    public class NcaInfoViewModel : FileViewModelBase
    {
        private readonly NcaHeader _ncaHeader;

        public NcaInfoViewModel(NcaHeader ncaHeader)
        {
            _ncaHeader = ncaHeader ?? throw new ArgumentNullException(nameof(ncaHeader));
        }

        public string TitleId => _ncaHeader.RawStruct.TitleId;

        public string Type => _ncaHeader.ContentType.ToString();

        public string DefinedSections
        {
            get
            {
                var definedSectionIndexes = _ncaHeader.DefinedSections.Select(sectionHeader => (int)sectionHeader.SectionIndex);
                return string.Join(",", definedSectionIndexes);
            }
        }

        public string SdkVersion => _ncaHeader.RawStruct.SdkVersion;

        public string CryptoType1 => _ncaHeader.RawStruct.CryptoType.ToString();

        public string CryptoType2 => _ncaHeader.RawStruct.CryptoType2.ToString();
    }
}