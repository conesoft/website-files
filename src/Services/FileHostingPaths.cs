using Conesoft.Files;

class FileHostingPaths(Conesoft.Hosting.HostEnvironment environment, IHttpContextAccessor httpContextAccessor, PublicFileHostingPaths publicPaths)
{
    public async Task<Conesoft.Files.Directory[]> GetRoots()
    {
        var paths = publicPaths.Paths.Select(Conesoft.Files.Directory.From).ToArray();

        if (httpContextAccessor?.HttpContext?.User.Identity?.Name is string name)
        {
            var personal = environment.Global.Storage / "Users" / name / Filename.From("conesoft-files", "json");
            if (personal.Exists && await personal.ReadFromJson<UserMappings>() is UserMappings mappings)
            {
                return [
                    .. paths,
                    .. mappings.Roots.Select(Conesoft.Files.Directory.From),
                    .. mappings.Mapped.Select(entry => new MappedDirectory(entry.Key, Conesoft.Files.Directory.From(entry.Value)))
                ];
            }
        }
        return paths;
    }

    record UserMappings(Dictionary<string, string> Mapped, string[] Roots);

    public record MappedDirectory(string MappedName, Conesoft.Files.Directory Root, bool InSubDirectory = false) : Conesoft.Files.Directory(Root.Parent)
    {
        public override string Name => InSubDirectory ? base.Name : MappedName;
        public override IEnumerable<Conesoft.Files.File> Files => InSubDirectory ? base.Files : [];

        public override IEnumerable<Conesoft.Files.Directory> Directories => InSubDirectory ? base.Directories : [Root];

        protected override Conesoft.Files.Directory SubDirectory(string subdirectory) => string.IsNullOrEmpty(subdirectory) ? this : base.SubDirectory(subdirectory);
    }
};