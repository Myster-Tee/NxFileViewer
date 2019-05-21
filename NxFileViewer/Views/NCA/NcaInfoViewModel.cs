using System;
using System.Linq;
using Emignatik.NxFileViewer.NSP.Models;
using Emignatik.NxFileViewer.Utils;
using LibHac.Fs.NcaUtils;

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

        public DistributionType DistributionType => _pfsNcaFile.DistributionType;

    }
}