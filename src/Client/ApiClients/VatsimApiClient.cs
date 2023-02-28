using System.Net.Http.Json;
using ZoaIds.Shared.ExternalDataModels;

namespace ZoaIds.Client.ApiClients;

public class VatsimApiClient
{
	private readonly HttpClient _httpClient;
	private readonly string _baseUri = Constants.ApiEndpoints.Vatsim;

	public VatsimApiClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<VatsimJsonRoot> GetDatafeed()
	{
		return await _httpClient.GetFromJsonAsync<VatsimJsonRoot>(_baseUri);
	}
}
