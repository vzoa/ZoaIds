using System.Text.Json.Serialization;

namespace ZoaIdsBackend.ExternalDataModels;

// ClowdApiDatis myDeserializedClass = JsonSerializer.Deserialize<List<ClowdApiDatis>>(myJsonResponse);
// Generated from https://json2csharp.com

public class ClowdApiDatis
{
    [JsonPropertyName("airport")]
    public string Airport { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("datis")]
    public string Datis { get; set; }
}

