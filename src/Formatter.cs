using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using RazorLight;

public class Formatter : IDirectoryFormatter
{
    private const string TextHtmlUtf8 = "text/html; charset=utf-8";
    private readonly RazorLightEngine engine = new RazorLightEngineBuilder().UseMemoryCachingProvider().Build();

    public virtual async Task GenerateContentAsync(HttpContext context, IEnumerable<IFileInfo> contents)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(contents);

        context.Response.ContentType = TextHtmlUtf8;

        if (HttpMethods.IsHead(context.Request.Method))
        {
            return;
        }

        var Template = "Templates/Files.cshtml";

        var model = new Model
        (
            Context: context,
            Contents: contents
        );

        var found = engine.Handler.Cache.RetrieveTemplate(Template);

        var response = found.Success ?
            await engine.RenderTemplateAsync(found.Template.TemplatePageFactory(), model)
            :
            await engine.CompileRenderStringAsync(Template, await File.ReadAllTextAsync(Template), model)
            ;

        await HttpResponseWritingExtensions.WriteAsync(context.Response, response);
    }

    public record Model(HttpContext Context, IEnumerable<IFileInfo> Contents);

}
