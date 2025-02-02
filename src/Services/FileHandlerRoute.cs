using Conesoft.ZipFolder;
using Microsoft.AspNetCore.StaticFiles;

namespace Conesoft.Website.Files.Services;

public static class FileHandlerRoute
{
    public static void MapFileHandlerRoute(this WebApplication app) => app.MapGet("/*/{*route}", (FileHostingPaths paths, string route, HttpContext context) =>
    {
        if (paths.DirectoryAt(route) is Conesoft.Files.Directory directory)
        {
            var directories = paths.DirectoriesAt(route).Select(d => d.Path);
            var name = route.Replace("/", " - ").Trim(' ', '-');

            context.Response.Headers.ContentLength = Zip.CalculateSize(directories);
            return Results.Stream(async s => await s.ZipSources(directories), contentType: "application/octet-stream", fileDownloadName: name + ".zip");
        }

        if (paths.FileAt(route) is Conesoft.Files.File file)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(file!.Name, out var contentType);
            contentType ??= file.Extension switch
            {
                ".mkv" => "video/webm", // evil but works.. :(
                _ => "application/octet-stream"
            };
            return Results.File(file.Path, contentType, file.Name, enableRangeProcessing: true);
        }

        return Results.NotFound();
    });
}