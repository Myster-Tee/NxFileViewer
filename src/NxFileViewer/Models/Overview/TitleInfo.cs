using LibHac.Ns;

namespace Emignatik.NxFileViewer.Models.Overview;

public class TitleInfo
{
    private readonly string _appName;
    private readonly string _publisher;

    /// <summary>
    /// Constructor for the 16 legacy languages sourced from LibHac's
    /// <c>ApplicationControlProperty.ApplicationTitle</c> struct.
    /// </summary>
    public TitleInfo(ref ApplicationControlProperty.ApplicationTitle applicationTitle, NacpLanguage language)
    {
        _appName   = applicationTitle.NameString.ToString();
        _publisher = applicationTitle.PublisherString.ToString();
        Language   = language;
    }

    /// <summary>
    /// Constructor for extended languages (index 16+) sourced from the compressed
    /// NACP title block, where title data is provided as plain strings.
    /// </summary>
    public TitleInfo(NacpTitleEntry titleEntry, NacpLanguage language)
    {
        _appName   = titleEntry.Name;
        _publisher = titleEntry.Publisher;
        Language   = language;
    }

    public string AppName => _appName;

    public string Publisher => _publisher;

    public NacpLanguage Language { get; }

    public byte[]? Icon { get; set; }

    public override string ToString()
    {
        var appName   = AppName;
        var publisher = Publisher;

        if (string.IsNullOrWhiteSpace(appName) && string.IsNullOrWhiteSpace(publisher))
            return "";

        var publisherStr = string.IsNullOrEmpty(publisher) ? "" : $" - {publisher}";
        return $"{appName}{publisherStr} ({Language})";
    }
}