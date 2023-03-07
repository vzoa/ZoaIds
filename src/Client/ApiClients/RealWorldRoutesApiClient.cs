using System.Net.Http.Json;
using ZoaIds.Shared.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace ZoaIds.Client.ApiClients;

public class RealWorldRoutesApiClient
{
	private readonly HttpClient _httpClient;
	private readonly string _baseUri = Constants.ApiEndpoints.RealWorldRoutes;

	public RealWorldRoutesApiClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<RealWorldRouting> GetRouteSummary(string departureIcaoId, string arrivalIcaoId)
	{
		var queryDict = new Dictionary<string, string>
		{ 
			["departure"] = departureIcaoId,
			["arrival"] = arrivalIcaoId
		};
		var uri = QueryHelpers.AddQueryString(_baseUri, queryDict);
		return await _httpClient.GetFromJsonAsync<RealWorldRouting>(uri);
	}

	public async Task<RealWorldRouting> GetRouteSummary(Airport departure, Airport arrival)
	{
		return await GetRouteSummary(departure.IcaoId, arrival.IcaoId);
	}
}