using Coravel;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ZoaIds.Server.Data;
using ZoaIds.Server.Jobs;
using ZoaIds.Server.Services;

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddHttpClient();


// Coravel-scheduled Jobs
builder.Services.AddScheduler();
builder.Services.AddTransient<FetchAndStoreAircraftIcao>();
builder.Services.AddTransient<FetchAndStoreAirlineIcao>();
builder.Services.AddTransient<FetchAndStoreMetars>();
builder.Services.AddTransient<FetchAndStoreNasrData>();
builder.Services.AddTransient<FetchAndStoreZoaDocs>();
builder.Services.AddTransient<DeleteOldVatsimSnapshots>();
builder.Services.AddTransient<DeleteOldRealWorldRoutes>();
builder.Services.AddTransient<FetchAndStoreAliasRoutes>();

// Constantly-running background services
builder.Services.AddHostedService<VatsimDataWorker>();
builder.Services.AddHostedService<DatisWorker>();
builder.Services.AddHostedService<RvrWorker>();

// Other Services
builder.Services.AddScoped<IRouteSummaryService, FlightAwareRouteService>();

// DB Context
builder.Services.AddPooledDbContextFactory<ZoaIdsContext>(
	//options => options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"))
	options => options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"))
);


var app = builder.Build();

// Coravel scheduler configuration
app.Services.UseScheduler(scheduler =>
{
	scheduler
		.Schedule<FetchAndStoreAirlineIcao>()
		.DailyAtHour(9)
		.RunOnceAtStart();

	scheduler
		.Schedule<FetchAndStoreAircraftIcao>()
		.DailyAt(9, 10)
		.RunOnceAtStart();

	scheduler
		.Schedule<FetchAndStoreNasrData>()
		.DailyAt(9, 15)
		.RunOnceAtStart();

	scheduler
		.Schedule<FetchAndStoreZoaDocs>()
		.DailyAt(9, 15)
		.RunOnceAtStart();

	scheduler
		.Schedule<FetchAndStoreAliasRoutes>()
		.DailyAt(9, 20)
		.RunOnceAtStart();

	scheduler
		.Schedule<DeleteOldVatsimSnapshots>()
		.Hourly()
		.RunOnceAtStart();

	scheduler
		.Schedule<DeleteOldRealWorldRoutes>()
		.Hourly()
		.RunOnceAtStart();

	scheduler
		.Schedule<FetchAndStoreMetars>()
		.Cron("*/6 * * * *") // every 6 minutes
		.RunOnceAtStart();
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}
else
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

var db = await app.Services.GetRequiredService<IDbContextFactory<ZoaIdsContext>>().CreateDbContextAsync();
//db.Database.EnsureDeleted();
db.Database.EnsureCreated();

app.Run();