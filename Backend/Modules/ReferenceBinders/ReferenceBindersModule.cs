﻿using ZoaIdsBackend.Common.Interfaces;
using ZoaIdsBackend.Modules.ReferenceBinders.Services;

namespace ZoaIdsBackend.Modules.ReferenceBinders;

public class ReferenceBindersModule : IServiceConfigurator
{
    public const string StaticPath = "/binders/static";
    public IServiceCollection AddServices(IServiceCollection services)
    {
        services.AddHostedService<ReferenceBindersBackgroundService>();
        return services;
    }
}
