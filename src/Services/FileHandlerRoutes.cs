using Conesoft.ZipFolder;
using Microsoft.AspNetCore.StaticFiles;

namespace Conesoft.Website.Files.Services;

public static class FileHandlerRoutes
{
    public static void MapFileHandlerRoutes(this WebApplication app)
    {
        app.MapGet("/*/{*route}", (FileHostingPaths paths, string route, HttpContext context) => MapFileHandlerRoute(paths, route, context, download: true));
        app.MapGet("/**/{*route}", (FileHostingPaths paths, string route, HttpContext context) => MapFileHandlerRoute(paths, route, context, download: false));
    }

    public static IResult MapFileHandlerRoute(FileHostingPaths paths, string route, HttpContext context, bool download)
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
            return Results.File(file.Path, contentType, download ? file.Name : null, enableRangeProcessing: download);
        }

        return Results.NotFound();
    }
}