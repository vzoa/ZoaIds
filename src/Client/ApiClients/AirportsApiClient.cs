using System.Net.Http.Json;
using ZoaIds.Shared.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace ZoaIds.Client.ApiClients;

public class AirportsApiClient
{
	private readonly HttpClient _httpClient;
	private readonly string _baseUri = Constants.ApiEndpoints.Airports;

	public AirportsApiClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<Airport> GetAirportByIcao(string airportIcaoId)
	{
		return await _httpClient.GetFromJsonAsync<Airport>($"{_baseUri}/{airportIcaoId}");
	}

	public async Task<Dictionary<string, Atis>> GetAirportByFaa(string airportFaaId)
	{
		var queryDict = new Dictionary<string, string> { ["idtype"] = "faa" };
		var uri = QueryHelpers.AddQueryString($"{_baseUri}/{airportFaaId}", queryDict);
		return await _httpClient.GetFromJsonAsync<Dictionary<string, Atis>>(uri);
	}

	public async Task<Dictionary<string, Atis>> GetAirportsByIcao(IEnumerable<string> airportsIcaoIds)
	{
		var airportIds = string.Join(",", airportsIcaoIds);
		return await _httpClient.GetFromJsonAsync<Dictionary<string, Atis>>($"{_baseUri}/{airportIds}");
	}

	public async Task<Dictionary<string, Atis>> GetAirportsByFaa(IEnumerable<string> airportsFaaIds)
	{
		var queryDict = new Dictionary<string, string> { ["idtype"] = "faa" };
		var airportsIds = string.Join(",", airportsFaaIds);
		var uri = QueryHelpers.AddQueryString($"{_baseUri}/{airportsIds}", queryDict);
		return await _httpClient.GetFromJsonAsync<Dictionary<string, Atis>>(uri);
	}
}