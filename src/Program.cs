using Conesoft.Hosting;
using Conesoft.Website.Files.Components;
using Conesoft.Website.Files.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpClient()
    .AddHttpContextAccessor()
    .AddSingleton(new PublicFileHostingPaths(@"D:\Public", @"E:\Public"))
    .AddScoped<FileHostingPaths>()
    .AddUsersWithStorage()
    .AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

app
    .UseStaticFiles()
    .UseRouting() // fixes routes for Scoped CSS as well as static files
    .UseAntiforgery();

app.MapUsersWithStorage();
app.MapFileHandlerRoute();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();