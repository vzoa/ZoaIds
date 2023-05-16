using Coravel;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using ZoaIdsBackend.Common;
using ZoaIdsBackend.Data;

namespace ZoaIdsBackend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.


        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add the global App Settings class to DI container
        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(AppSettings.SectionKeyName));

        // Add DB context
        builder.Services.AddPooledDbContextFactory<ZoaIdsContext>(options => options
            .UseSqlite(builder.Configuration.GetConnectionString("Sqlite"))
        );

        builder.Services.AddHttpClient();

        // Response caching
        builder.Services.AddResponseCaching();


        builder.Services.AddMemoryCache();


        // Use Scrutor to scan and register services with DI container
        //builder.Services.Scan(scan => scan
        //    .FromCallingAssembly()
        //        .AddClasses(classes => classes.AssignableTo<IHostedService>())
        //            .As<IHostedService>()
        //            .WithSingletonLifetime()
        //        .AddClasses(classes => classes.AssignableTo<IInvocable>())
        //            .AsSelf()
        //            .WithTransientLifetime()
        //);

        // Scan for and add modules
        builder.Services.AddModuleServices();

        // Add Fast Endpoints
        builder.Services.AddFastEndpoints();

        // Add Coravel scheduler
        builder.Services.AddScheduler();


        builder.Services.AddCors();


        var app = builder.Build();

        // Scan for and add schedulers
        app.Services.UseSchedulers();



        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        // Response caching
        app.UseResponseCaching();

        app.UseAuthorization();


        if (app.Environment.IsDevelopment())
        {
            app.UseCors(b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        }

        app.UseDefaultExceptionHandler();
        app.UseFastEndpoints(c =>
        {
            c.Endpoints.RoutePrefix = "api";
            c.Versioning.Prefix = "v";
            c.Versioning.PrependToRoute = true;
            c.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
        });


        var dbContextFactory = app.Services.GetRequiredService<IDbContextFactory<ZoaIdsContext>>();
        using var db = dbContextFactory.CreateDbContext();
        db.Database.EnsureCreated();

        app.Run();
    }
}