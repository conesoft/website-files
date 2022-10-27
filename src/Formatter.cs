using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using RazorLight;

public class Formatter : IDirectoryFormatter
{
    private const string TextHtmlUtf8 = "text/html; charset=utf-8";

    public virtual async Task GenerateContentAsync(HttpContext context, IEnumerable<IFileInfo> contents)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(contents);

        context.Response.ContentType = TextHtmlUtf8;

        if (HttpMethods.IsHead(context.Request.Method))
        {
            return;
        }

        var engine = new RazorLightEngineBuilder()
            .SetOperatingAssembly(typeof(Model).Assembly)
            .Build();

        var template = new
        {
            Name = "Demo",
            Content = await File.ReadAllTextAsync("Templates/Files.cshtml")
        };

        var model = new Model
        (
            Context: context,
            Contents: contents
        );

        var results = await engine.CompileRenderStringAsync(template.Name, template.Content, model);

        await HttpResponseWritingExtensions.WriteAsync(context.Response, results);
    }

    public record Model( HttpContext Context, IEnumerable<IFileInfo> Contents);

}
