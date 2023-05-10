using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ZoaIdsBackend.Data;

namespace ZoaIdsBackend.Modules.VatsimData.Repositories;

public class VatsimDataRepositoryFactory
{
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;
    private readonly IMemoryCache _cache;

    public VatsimDataRepositoryFactory(IDbContextFactory<ZoaIdsContext> contextFactory, IMemoryCache cache)
    {
        _contextFactory = contextFactory;
        _cache = cache;
    }

    public IVatsimDataRepository CreateVatsimDataRepository()
    {
        return new CachedVatsimDataRepository(_contextFactory, _cache);
    }
}
