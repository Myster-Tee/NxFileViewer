using LibHac.Ns;

namespace Emignatik.NxFileViewer.Model.Overview
{
    public class TitleInfo
    {
        private readonly ApplicationControlTitle _description;

        public TitleInfo(ApplicationControlTitle description, NacpLanguage language)
        {
            _description = description;
            Language = language;
        }

        public string AppName => _description.Name.ToString();

        public string Publisher => _description.Publisher.ToString();

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