using System.Text.Json;
using System.Text.Json.Serialization;

namespace ZoaIdsBackend.Modules.VatsimData.Models;

public class VatspyBoundariesGeoJson
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class Crs
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("properties")]
        public Properties Properties { get; set; }
    }

    public class Feature
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("properties")]
        public Properties Properties { get; set; }

        [JsonPropertyName("geometry")]
        public Geometry Geometry { get; set; }
    }

    public class Geometry
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("coordinates")]
        public List<List<List<List<double>>>> Coordinates { get; set; }
    }

    public class Properties
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("oceanic")]
        [JsonConverter(typeof(BoolConverter))]
        public bool IsOceanic { get; set; }

        [JsonPropertyName("label_lon")]
        public double LabelLongitude { get; set; }

        [JsonPropertyName("label_lat")]
        public double LabelLatitude { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonPropertyName("division")]
        public string Division { get; set; }
    }

    public class Root
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("crs")]
        public Crs Crs { get; set; }

        [JsonPropertyName("features")]
        public List<Feature> Features { get; set; }
    }

    private class BoolConverter : JsonConverter<bool>
    {
        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) =>
            writer.WriteBooleanValue(value);

        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            reader.TokenType switch
            {
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                JsonTokenType.String => TryParseWithNumbers(reader.GetString(), out var b) ? b : throw new JsonException(),
                JsonTokenType.Number => reader.TryGetInt64(out long l) ? Convert.ToBoolean(l) : reader.TryGetDouble(out double d) ? Convert.ToBoolean(d) : false,
                _ => throw new JsonException(),
            };

        private static bool TryParseWithNumbers(string s, out bool b)
        {
            if (bool.TryParse(s, out b))
            {
                return true;
            }
            else
            {
                try
                {
                    b = s.Trim() == "1";
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
