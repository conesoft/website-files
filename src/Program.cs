using Microsoft.Extensions.FileProviders;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDirectoryBrowser();

var app = builder.Build();

var options = new FileServerOptions
{
    EnableDirectoryBrowsing = true,
    FileProvider = new PhysicalFileProvider(@"E:\Public")
};
options.StaticFileOptions.ServeUnknownFileTypes = true;
options.StaticFileOptions.RedirectToAppendTrailingSlash = false;
options.DirectoryBrowserOptions.RedirectToAppendTrailingSlash = false;
options.DirectoryBrowserOptions.Formatter = new Formatter();
app.UseFileServer(options);

app.MapGet("/download-as-file/{*path}", async (string path, HttpContext context) =>
{
    var root = @"E:\Public\";
    var directory = root + path.Replace("/", "\\");

    context.Response.StatusCode = StatusCodes.Status200OK;

    if (File.Exists(directory))
    {
        var file = directory;

        context.Response.ContentType = "application/octet-stream";
        context.Response.ContentLength = new FileInfo(file).Length;
        context.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{Path.GetFileName(file)}\"");

        using var fileStream = System.IO.File.OpenRead(file);
        await fileStream.CopyToAsync(context.Response.BodyWriter.AsStream());
        return;
    }

    if (Directory.Exists(directory) == false)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }

    if ((new Uri(root)).IsBaseOf(new Uri(directory)) == false)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return;
    }

    var parent = Directory.GetParent(directory);
    var name = parent?.Name ?? "error";
    if (parent != null && parent.Name.StartsWith("Season "))
    {
        parent = parent.Parent;
        name = parent?.Name + " - " + name;
    }

    var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);

    context.Response.ContentType = "application/octet-stream";
    if(PositionWrapperStream.MeasureSizeWhenZipped(files, maxFiles: 20) is long length)
    {
        context.Response.ContentLength = length;
    }
    context.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{name}.zip\"");

    using ZipArchive archive = new(context.Response.BodyWriter.AsStream(), ZipArchiveMode.Create);
    foreach (var file in files)
    {
        archive.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.NoCompression);
    }
});

app.Run();
