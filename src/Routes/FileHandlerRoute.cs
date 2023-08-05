using Microsoft.AspNetCore.StaticFiles;

namespace Conesoft.Website.Files.Routes;

public static class FileHandlerRoute
{
    public static void MapFileHandlerRoute(this WebApplication app)
    {
        app.MapGet("/*/{*route:regex(.*[^/]$)}", (FileHostingPath path, string route) =>
        {
            var file = (path.Directory / route).AsFile;
            if(file.Exists == false)
            {
                return Results.Redirect(route + "/");
            }

            new FileExtensionContentTypeProvider().TryGetContentType(file.Name, out var contentType);
            contentType ??= file.Extension switch
            {
                ".mkv" => "video/webm", // evil but works.. :(
                _ => "application/octet-stream"
            };

            return Results.File(file.Path, contentType, enableRangeProcessing: true);
        });
    }
}