using System.Net.Http.Json;
using ZoaIds.Shared.Models;

namespace ZoaIds.Client.ApiClients;

public class DatisApiClient
{
	private readonly HttpClient _httpClient;
	private readonly string _baseUri = Constants.ApiEndpoints.Datis;

	public DatisApiClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<Dictionary<string, Atis>> GetAllDatis()
	{
		return await _httpClient.GetFromJsonAsync<Dictionary<string, Atis>>(_baseUri);
	}

	public async Task<Atis> GetAirportAtis(string airportIcaoId)
	{
		return await _httpClient.GetFromJsonAsync<Atis>($"{_baseUri}/{airportIcaoId}");
	}

	public async Task<Dictionary<string, Atis>> GetManyAirportsAtis(IEnumerable<string> airportsIcaoIds)
	{
		var airportsIds = string.Join(",", airportsIcaoIds);
		return await _httpClient.GetFromJsonAsync<Dictionary<string, Atis>>($"{_baseUri}/{airportsIds}");
	}

	public async Task<Dictionary<string, Atis>> GetManyAirportsAtis(IEnumerable<Airport> airports)
	{
		var airportsIds = airports.Select(a => a.IcaoId);
		return await GetManyAirportsAtis(airportsIds);
	}
}