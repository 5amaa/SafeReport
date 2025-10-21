using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SafeReport.Web;
using SafeReport.Web.Extensions;
using SafeReport.Web.Interfaces;
using SafeReport.Web.Services;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped<IReportService, ReportService>();
// Register HttpClient for WebAssembly
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.RootComponents.Add<App>("#app");
builder.Services.AddLocalization(options => options.ResourcesPath = "ResourcesFiles");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var host = builder.Build();
await host.EnsureDefaultCultureAsync();
await host.RunAsync();
//await builder.Build().RunAsync();
