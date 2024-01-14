using System.Text.Json.Serialization;

namespace Emignatik.NxFileViewer.Services.OnlineServices;

public class OnlineTitleInfo : IOnlineTitleInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("publisher")]
    public string Publisher { get; set; } = null!;

    [JsonPropertyName("icon_url")]
    public string IconUrl { get; set; } = null!;

    [JsonPropertyName("description")]
    public string Description { get; set; } = null!;

    [JsonPropertyName("playtime")]
    public object Playtime { get; set; } = null!;

    [JsonPropertyName("rating")]
    public double Rating { get; set; }

}