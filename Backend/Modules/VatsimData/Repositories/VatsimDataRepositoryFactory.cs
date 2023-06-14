using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ZoaIdsBackend.Data;

namespace ZoaIdsBackend.Modules.VatsimData.Repositories;

public class VatsimDataRepositoryFactory
{
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;
    private readonly IMemoryCache _cache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptionsMonitor<AppSettings> _appSettings;

    public VatsimDataRepositoryFactory(IDbContextFactory<ZoaIdsContext> contextFactory, IMemoryCache cache, IHttpClientFactory httpClientFactory, IOptionsMonitor<AppSettings> appSettings)
    {
        _contextFactory = contextFactory;
        _cache = cache;
        _httpClientFactory = httpClientFactory;
        _appSettings = appSettings;
    }

    public IVatsimDataRepository CreateVatsimDataRepository()
    {
        return new CachedVatsimDataRepository(_contextFactory, _cache, _httpClientFactory, _appSettings);
    }
}
