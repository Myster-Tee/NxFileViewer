namespace Emignatik.NxFileViewer.NSP.Models
{
    /// <summary>
    /// Represents localized information of an application
    /// </summary>
    public class TitleInfo
    {
        public string AppName { get; set; }

        public string Publisher { get; set; }

        public NacpLanguage Language { get; set; }

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