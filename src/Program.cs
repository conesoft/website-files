using Conesoft.Hosting;
using Conesoft.PwaGenerator;
using Conesoft.Website.Files.Components;
using Conesoft.Website.Files.Services;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddHostConfigurationFiles()
    .AddHostEnvironmentInfo()
    .AddLoggingService()
    .AddUsersWithStorage()
    ;

builder.Services
    .AddCompiledHashCacheBuster()
    .AddViewTransition()
    .AddHttpClient()
    .AddHttpContextAccessor()
    .AddSingleton(new PublicFileHostingPaths(@"D:\Public", @"E:\Public"))
    .AddScoped<FileHostingPaths>()
    .AddRazorComponents().AddInteractiveServerComponents()
    ;

var app = builder.Build();

app.MapPwaInformationFromAppSettings();
app.MapStaticAssets();
app.MapUsersWithStorage();
app.MapFileHandlerRoute();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();