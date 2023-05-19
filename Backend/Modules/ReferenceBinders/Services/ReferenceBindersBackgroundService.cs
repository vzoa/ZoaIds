using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ZoaIdsBackend.Modules.ReferenceBinders.Models;

namespace ZoaIdsBackend.Modules.ReferenceBinders.Services;

public class ReferenceBindersBackgroundService : BackgroundService
{
    private readonly ILogger<ReferenceBindersBackgroundService> _logger;
    private readonly IOptionsMonitor<AppSettings> _appSettings;
    private readonly IMemoryCache _cache;
    public const string CacheKey = "ReferenceBinderStructure";

    public ReferenceBindersBackgroundService(ILogger<ReferenceBindersBackgroundService> logger, IOptionsMonitor<AppSettings> appSettings, IMemoryCache cache)
    {
        _logger = logger;
        _appSettings = appSettings;
        _cache = cache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Scanning reference binders directory: {path}", _appSettings.CurrentValue.ReferenceBindersRootDirectory);
                var fetchedBinder = CreateBinder(_appSettings.CurrentValue.ReferenceBindersRootDirectory);
                _cache.Set(CacheKey, fetchedBinder);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while scanning reference binders directory: {ex}", ex.ToString());
            }
            
            await Task.Delay(1000 * _appSettings.CurrentValue.BindersRefreshSeconds, stoppingToken);
        }
    }

    private static Binder CreateBinder(string fullPath, string parentPath = "")
    {
        if (string.IsNullOrEmpty(parentPath)) {
            parentPath = fullPath;
        }

        var dir = new DirectoryInfo(fullPath);
        var binder = new Binder(MakeRelativePath(fullPath, parentPath));
        foreach(var childDir in dir.EnumerateDirectories())
        {
            binder.Children.Add(CreateBinder(childDir.FullName, dir.FullName));
        }
        foreach (var childFile in dir.EnumerateFiles())
        {
            binder.Children.Add(new Document(MakeRelativePath(childFile.FullName, dir.FullName)));
        }       
        return binder;
    }

    private static string MakeRelativePath(string fullPath, string basePath)
    {
        var fullUri = new Uri(fullPath, UriKind.Absolute);
        var baseUri = new Uri(basePath, UriKind.Absolute);
        return baseUri.MakeRelativeUri(fullUri).ToString();
    }
}
