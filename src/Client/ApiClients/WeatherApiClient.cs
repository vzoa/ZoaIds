using System.Net.Http.Json;
using ZoaIds.Shared.ExternalDataModels;
using ZoaIds.Shared.Models;

namespace ZoaIds.Client.ApiClients;

public class WeatherApiClient
{
	private readonly HttpClient _httpClient;
	private readonly string _baseUri = Constants.ApiEndpoints.Weather;

	public WeatherApiClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<Metar> GetMetar(string airportIcaoId)
	{
		return await _httpClient.GetFromJsonAsync<Metar>($"{_baseUri}/metar/{airportIcaoId}");
	}

	public async Task<Metar> GetMetar(Airport airport)
	{
		return await GetMetar(airport.IcaoId);
	}
}
