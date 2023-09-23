using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.StaticFiles;
using System.Runtime.CompilerServices;

namespace Conesoft.Website.Files.Routes;

public static class FileHandlerRoute
{
    public static void MapFileHandlerRoute(this WebApplication app)
    {
        //        app.MapGet("/*/{*route:regex(.*[^/]$)}", (FileHostingPath path, string route) =>
        app.MapGet("/*/{*route}", async (FileHostingPath path, string route, HttpContext context) =>
        {
            var file = (path.Directory / route).AsFile;
            var directory = (path.Directory / route);
            if (file.Exists == false && directory.Exists == false)
            {
                return Results.NotFound();
            }

            if (file.Exists == false && directory.Exists == true)
            {
                if (context.Features.Get<IHttpBodyControlFeature>() is var syncIOFeature and not null)
                {
                    syncIOFeature.AllowSynchronousIO = true;
                }

                var response = context.Response;

                var zipFolder = new ZipFolder.FolderToZip(directory.Path);

                var name = directory.Path.Replace(@"E:\Public\", "").Replace("/", " - ").Trim(' ', '-');

                response.Headers.ContentLength = zipFolder.ContentLength;
                response.Headers.ContentDisposition = $"attachment; filename=\"{name}.zip\"";
                response.Headers.ContentType = "application/octet-stream";

                zipFolder.StreamTo(response.Body);

                return Results.Empty;
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