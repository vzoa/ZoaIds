namespace ZoaIdsBackend.Common.Interfaces;

public interface IServiceConfigurator
{
    IServiceCollection AddServices(IServiceCollection builder);
}
