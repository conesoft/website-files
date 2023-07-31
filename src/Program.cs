using Conesoft.Users;
using Conesoft.Website.Files.Routes;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(new FileHostingPath(Path: @"E:\Public"));
builder.Services.AddUsers("Conesoft.Host.User", (Conesoft.Hosting.Host.GlobalStorage / "Users").Path);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapUsers();

app.MapFileHandlerRoute(); 
app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub(options =>
{
    options.WebSockets.CloseTimeout = TimeSpan.MaxValue;
});
app.MapFallbackToPage("/_Host");

app.Run();
