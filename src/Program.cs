using Conesoft.Hosting;
using Conesoft.PwaGenerator;
using Conesoft.Website.Files.Components;
using Conesoft.Website.Files.Services;
using System.Reflection;
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
    .AddSingleton<CategoryManager>()
    .AddHttpContextAccessor()
    .AddRazorComponents().AddInteractiveServerComponents()
    ;

var app = builder.Build();

app.MapPwaInformationFromAppSettings();
app.MapStaticAssets();
app.MapUsersWithStorage();
app.MapFileHandlerRoutes();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();

public static class ObjectExtensions
{
    public static IDictionary<string, object?> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
    {
        return source.GetType().GetProperties(bindingAttr).ToDictionary(propInfo => propInfo.Name, propInfo => propInfo.GetValue(source, null)) ?? [];
    }
}
