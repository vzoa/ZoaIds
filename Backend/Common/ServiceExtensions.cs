using Coravel;
using ZoaIdsBackend.Common.Interfaces;

namespace ZoaIdsBackend.Common;

public static class ServiceExtensions
{
    public static IServiceProvider UseSchedulers(this IServiceProvider provider)
    {
        var configurators = DiscoverClassesOfType<ISchedulerConfigurator>();
        foreach (var configurator in configurators)
        {
            provider.UseScheduler(configurator.ConfigureScheduler());
        }
        return provider;
    }

    public static IServiceCollection AddModuleServices(this IServiceCollection collection)
    {
        var configurators = DiscoverClassesOfType<IServiceConfigurator>();
        foreach (var configurator in configurators)
        {
            configurator.AddServices(collection);
        }
        return collection;
    }

    private static IEnumerable<T> DiscoverClassesOfType<T>()
    {
        return typeof(T).Assembly
            .GetTypes()
            .Where(p => p.IsClass && p.IsAssignableTo(typeof(T)))
            .Select(Activator.CreateInstance)
            .Cast<T>();
    }
}
