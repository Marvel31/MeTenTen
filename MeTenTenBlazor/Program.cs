using MeTenTenBlazor;
using MeTenTenBlazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<MeTenTenBlazor.Components.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add services for Vercel deployment (local storage based)
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<ITopicService, LocalTopicService>();
builder.Services.AddScoped<ITenTenService, LocalTenTenService>();

// Add HttpClient
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
