using System;
using System.Windows.Media.Imaging;
using LibHac;

namespace Emignatik.NxFileViewer.Model.Overview
{
    public class TitleInfo
    {
        private readonly NacpDescription _description;

        public TitleInfo(NacpDescription description)
        {
            _description = description ?? throw new ArgumentNullException(nameof(description));
        }

        public string AppName => _description.Title;

        public string Publisher => _description.Developer;

        public NacpLanguage Language => (NacpLanguage) _description.Language;

        public BitmapImage? Icon { get; set; }

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