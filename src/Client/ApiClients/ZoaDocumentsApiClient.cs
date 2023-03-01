using System.Net.Http.Json;
using ZoaIds.Shared.Models;

namespace ZoaIds.Client.ApiClients;

public class ZoaDocumentsApiClient
{
	private readonly HttpClient _httpClient;
	private readonly string _baseUri = Constants.ApiEndpoints.ZoaDocuments;

	public ZoaDocumentsApiClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<List<ArtccDocument>> GetAllDocuments()
	{
		return await _httpClient.GetFromJsonAsync<List<ArtccDocument>>(_baseUri);
	}
}
