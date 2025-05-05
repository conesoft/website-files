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
    .AddCompiledHashCacheBuster()
    .AddHostingDefaults()
    ;

builder.Services
    .AddViewTransition()
    .AddHttpContextAccessor()
    .AddSingleton(new PublicFileHostingPaths(@"D:\Public", @"E:\Public"))
    .AddScoped<FileHostingPaths>()
    .AddSingleton<CategoryManager>()
    .AddHttpContextAccessor()
    ;

var app = builder.Build();

app.UseCompiledHashCacheBuster();
app.MapPwaInformationFromAppSettings();
app.MapStaticAssets();
app.MapUsersWithStorage();
app.MapFileHandlerRoutes();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();