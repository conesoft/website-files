using Conesoft.Hosting;
using Conesoft.PwaGenerator;
using Conesoft.Website.Files.Components;
using Conesoft.Website.Files.Services;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddHostConfigurationFiles()
    .AddHostEnvironmentInfo()
    .AddLoggingService()
    .AddUsersWithStorage()
    ;

builder.Services
    .AddCompiledHashCacheBuster()
    .AddHttpClient()
    .AddHttpContextAccessor()
    .AddSingleton(new PublicFileHostingPaths(@"D:\Public", @"E:\Public"))
    .AddScoped<FileHostingPaths>()
    .AddRazorComponents().AddInteractiveServerComponents().AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
        options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromSeconds(0);
        options.DisconnectedCircuitMaxRetained = 0;
    });

var app = builder.Build();

app
    .UseCompiledHashCacheBuster()
    .UseRouting() // fixes routes for Scoped CSS as well as static files
    .UseAntiforgery();

app.MapUsersWithStorage();

app.MapPwaInformationFromAppSettings();

app.MapStaticAssets();
app.MapFileHandlerRoute();
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode()
;

app.Run();