using System.Net.Http.Json;
using ZoaIds.Shared.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace ZoaIds.Client.ApiClients;

public class AliasRoutesApiClient
{
	private readonly HttpClient _httpClient;
	private readonly string _baseUri = Constants.ApiEndpoints.AliasRoutes;

	public AliasRoutesApiClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<ICollection<RouteRule>> GetRouteRules(string departureIcaoId, string arrivalIcaoId)
	{
		var queryDict = new Dictionary<string, string>
		{ 
			["departure"] = departureIcaoId,
			["arrival"] = arrivalIcaoId
		};
		var uri = QueryHelpers.AddQueryString(_baseUri, queryDict);
		return await _httpClient.GetFromJsonAsync<ICollection<RouteRule>>(uri);
	}

	public async Task<ICollection<RouteRule>> GetRouteRules(Airport departure, Airport arrival)
	{
		return await GetRouteRules(departure.IcaoId, arrival.IcaoId);
	}
}