using System.Net.Http.Json;
using ZoaIds.Shared.ExternalDataModels;
using ZoaIds.Shared.Models;

namespace ZoaIds.Client.ApiClients;

public class ChartsApiClient
{
	private readonly HttpClient _httpClient;
	private readonly string _baseUri = Constants.ApiEndpoints.Charts;

	public ChartsApiClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<Dictionary<string, List<AviationApiChart>>> GetAirportCharts(string airportIcaoId)
	{
		return await _httpClient.GetFromJsonAsync<Dictionary<string, List<AviationApiChart>>>($"{_baseUri}/{airportIcaoId}");
	}

	public async Task<Dictionary<string, List<AviationApiChart>>> GetAirportCharts(Airport airport)
	{
		return await GetAirportCharts(airport.IcaoId);
	}
}
