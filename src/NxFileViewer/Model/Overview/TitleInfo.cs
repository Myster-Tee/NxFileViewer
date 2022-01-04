using LibHac.Ns;

namespace Emignatik.NxFileViewer.Model.Overview
{
    public class TitleInfo
    {
        private readonly ApplicationControlTitle _applicationControlTitle;

        public TitleInfo(ref ApplicationControlTitle applicationControlTitle, NacpLanguage language)
        {
            _applicationControlTitle = applicationControlTitle;
            Language = language;
        }

        public string AppName => _applicationControlTitle.Name.ToString();

        public string Publisher => _applicationControlTitle.Publisher.ToString();

        public NacpLanguage Language { get; }

        public byte[]? Icon { get; set; }

        public override string ToString()
        {
            var appName = AppName;
            var publisher = Publisher;

            if (string.IsNullOrWhiteSpace(appName) && string.IsNullOrWhiteSpace(publisher)) return "";

            var publisherStr = string.IsNullOrEmpty(publisher) ? "" : $" - {publisher}";
            return $"{appName}{publisherStr} ({Language})";
        }
    }
}