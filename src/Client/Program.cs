using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using ZoaIds.Client;
using ZoaIds.Client.ApiClients;
using ZoaIds.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();

// Global State Container
// Scoped service acts like singleton for Blazor WASM
builder.Services.AddScoped<StateContainer>();

// Vatsim Datafeed global datafeed refresh service
// Scoped service acts like singleton for Blazor WASM
builder.Services.AddScoped<VatsimDatafeedUpdater>();

// Injectable API endpoint-specific HTTP clients with the base address set to the API base (default is "api/v1/")
var apiBaseUri = new Uri(builder.HostEnvironment.BaseAddress + Constants.ApiBase);
builder.Services.AddHttpClient<VatsimApiClient>(client => client.BaseAddress = apiBaseUri);
builder.Services.AddHttpClient<DatisApiClient>(client => client.BaseAddress = apiBaseUri);
builder.Services.AddHttpClient<ChartsApiClient>(client => client.BaseAddress = apiBaseUri);
builder.Services.AddHttpClient<ZoaDocumentsApiClient>(client => client.BaseAddress = apiBaseUri);
builder.Services.AddHttpClient<AirportsApiClient>(client => client.BaseAddress = apiBaseUri);
builder.Services.AddHttpClient<WeatherApiClient>(client => client.BaseAddress = apiBaseUri);

await builder.Build().RunAsync();