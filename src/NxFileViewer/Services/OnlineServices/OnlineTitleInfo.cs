using System.Text.Json.Serialization;

namespace Emignatik.NxFileViewer.Services.OnlineServices;

public class OnlineTitleInfo : IOnlineTitleInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("publisher")]
    public string Publisher { get; set; }

    [JsonPropertyName("icon_url")]
    public string IconUrl { get; set; }
    
    [JsonPropertyName("description")]
    public string Description { get; set; } 
    
    [JsonPropertyName("playtime")]
    public object Playtime { get; set; }

    [JsonPropertyName("rating")]
    public double Rating { get; set; }

}