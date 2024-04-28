using Conesoft.Files;

class FileHostingPaths(IHttpContextAccessor httpContextAccessor, PublicFileHostingPaths publicPaths)
{
    public async Task<Conesoft.Files.Directory[]> GetRoots()
    {
        var paths = publicPaths.Paths.Select(Conesoft.Files.Directory.From).ToArray();

        if(httpContextAccessor?.HttpContext?.User.Identity?.Name is string name)
        {
            var personal = Conesoft.Hosting.Host.GlobalStorage / "Users" / name / Filename.From("conesoft-files", "txt");
            if(personal.Exists && await personal.ReadLines() is string[] lines)
            {
                return [.. paths, .. lines.Select(Conesoft.Files.Directory.From)];
            }
        }
        return paths;
    }
};