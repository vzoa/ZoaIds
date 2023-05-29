using FastEndpoints;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Mime;
using System.Text.Json;
using ZoaIdsBackend.Modules.ReferenceBinders.Models;
using ZoaIdsBackend.Modules.ReferenceBinders.Services;

namespace ZoaIdsBackend.Modules.ReferenceBinders.Endpoints;

public class GetReferenceBindersHierarchy : EndpointWithoutRequest<INode>
{
    private readonly IMemoryCache _cache;
    private readonly JsonSerializerOptions _serializerOptions;

    public GetReferenceBindersHierarchy(IMemoryCache cache)
    {
        _cache = cache;
        _serializerOptions = new JsonSerializerOptions
        {
            Converters = { new DocumentConverter(), new BinderConverter() }
        };
    }

    public override void Configure()
    {
        Get("/binders");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        if (_cache.TryGetValue(ReferenceBindersBackgroundService.CacheKey, out var binder))
        {
            if (binder is not null and Binder)
            {
                var str = JsonSerializer.Serialize((Binder)binder, _serializerOptions);
                await SendStringAsync(str, contentType: MediaTypeNames.Application.Json);
            }
        }
    }
}
