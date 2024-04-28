using Conesoft.ZipFolder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.StaticFiles;

namespace Conesoft.Website.Files.Services;

public static class FileHandlerRoute
{
    public static void MapFileHandlerRoute(this WebApplication app)
    {
        //        app.MapGet("/*/{*route:regex(.*[^/]$)}", (FileHostingPath path, string route) =>
        app.MapGet("/*/{*route}", (FileHostingPaths paths, string route, HttpContext context) =>
        {
            var file = paths.Roots.Select(p => (p / route).AsFile).FirstOrDefault(f => f.Exists);
            var directory = paths.Roots.Select(p => p / route).FirstOrDefault(p => p.Exists);
            if (file != null && file.Exists == false && (directory == null || directory.Exists == false))
            {
                return Results.NotFound();
            }
            if ((file == null || file.Exists == false) && (directory == null || directory.Exists == false))
            {
                return Results.NotFound();
            }
            if ((file == null || file.Exists == false) && directory != null && directory.Exists == true)
            {
                if (context.Features.Get<IHttpBodyControlFeature>() is var syncIOFeature and not null)
                {
                    syncIOFeature.AllowSynchronousIO = true;
                }

                var response = context.Response;

                var name = directory.Path.Replace("\\", "/");
                foreach (var p in paths.Roots.Select(d => d.Path.Replace("\\", "/") + "/"))
                {
                    name = name.Replace(p, "");
                }
                name = name.Replace("/", " - ").Trim(' ', '-');

                var directories = paths.Roots.Select(p => p / route).Where(p => p.Exists).Select(p => p.Path);

                response.Headers.ContentLength = Zip.CalculateSize(directories);
                response.Headers.ContentDisposition = $"attachment; filename=\"{name}.zip\"";
                response.Headers.ContentType = "application/octet-stream";

                response.Body.ZipSources(directories);

                return Results.Empty;
            }


            new FileExtensionContentTypeProvider().TryGetContentType(file!.Name, out var contentType);
            contentType ??= file.Extension switch
            {
                ".mkv" => "video/webm", // evil but works.. :(
                _ => "application/octet-stream"
            };

            return Results.File(file.Path, contentType, enableRangeProcessing: true);
        });
    }
}