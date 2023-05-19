using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ZoaIdsBackend.Modules.ReferenceBinders.Models;

namespace ZoaIdsBackend.Modules.ReferenceBinders.Services;

public class ReferenceBindersBackgroundService : BackgroundService
{
    private readonly ILogger<ReferenceBindersBackgroundService> _logger;
    private readonly IOptionsMonitor<AppSettings> _appSettings;
    private readonly IWebHostEnvironment _environment;
    private readonly IMemoryCache _cache;
    public const string CacheKey = "ReferenceBinderStructure";

    public ReferenceBindersBackgroundService(ILogger<ReferenceBindersBackgroundService> logger, IOptionsMonitor<AppSettings> appSettings, IWebHostEnvironment environment, IMemoryCache cache)
    {
        _logger = logger;
        _appSettings = appSettings;
        _environment = environment;
        _cache = cache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var path = Path.Combine(_environment.WebRootPath, _appSettings.CurrentValue.ReferenceBindersDirectoryInWwwroot);
                _logger.LogInformation("Scanning reference binders directory: {path}", path);
                var fetchedBinder = CreateBinder(path);
                _cache.Set(CacheKey, fetchedBinder);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while scanning reference binders directory: {ex}", ex.ToString());
            }
            
            await Task.Delay(1000 * _appSettings.CurrentValue.BindersRefreshSeconds, stoppingToken);
        }
    }

    private Binder CreateBinder(string currentPath, string rootPath = "")
    {
        if (string.IsNullOrEmpty(rootPath)) {
            rootPath = currentPath;
        }

        var dir = new DirectoryInfo(currentPath);
        var binder = new Binder(Uri.UnescapeDataString(MakeRelativePath(currentPath, rootPath)));
        foreach(var childDir in dir.EnumerateDirectories())
        {
            binder.Children.Add(CreateBinder(childDir.FullName, rootPath));
        }
        foreach (var childFile in dir.EnumerateFiles("*.md"))
        {
            var relativePath = Uri.UnescapeDataString(MakeRelativePath(childFile.FullName, rootPath));
            binder.Children.Add(new Document(relativePath, MakeUrl(relativePath)));
        }       
        return binder;
    }

    private static string MakeRelativePath(string fullPath, string basePath)
    {
        var fullUri = new Uri(fullPath, UriKind.Absolute);
        var baseUri = new Uri(basePath, UriKind.Absolute);
        return baseUri.MakeRelativeUri(fullUri).ToString();
    }

    private string MakeUrl(string relativePath)
    {
        return new Uri(new Uri(_appSettings.CurrentValue.Urls.AppBase), $"{ReferenceBindersModule.StaticPath}/{relativePath}").AbsoluteUri;
    }
}
