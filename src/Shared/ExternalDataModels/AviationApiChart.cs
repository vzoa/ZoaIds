using System.Text.Json.Serialization;

namespace ZoaIds.Shared.ExternalDataModels;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
// Generated from https://json2csharp.com/

public class AviationApiChart
{
	[JsonPropertyName("state")]
	public string State { get; set; }

	[JsonPropertyName("state_full")]
	public string StateFull { get; set; }

	[JsonPropertyName("city")]
	public string City { get; set; }

	[JsonPropertyName("volume")]
	public string Volume { get; set; }

	[JsonPropertyName("airport_name")]
	public string AirportName { get; set; }

	[JsonPropertyName("military")]
	public string Military { get; set; }

	[JsonPropertyName("faa_ident")]
	public string FaaId { get; set; }

	[JsonPropertyName("icao_ident")]
	public string IcaoId { get; set; }

	[JsonPropertyName("chart_seq")]
	public string ChartSequence { get; set; }

	[JsonPropertyName("chart_code")]
	public string ChartCode { get; set; }

	[JsonPropertyName("chart_name")]
	public string ChartName { get; set; }

	[JsonPropertyName("pdf_name")]
	public string PdfName { get; set; }

	[JsonPropertyName("pdf_path")]
	public string PdfPath { get; set; }
}
