using Conesoft.Files;
using Serilog;

namespace Conesoft.Website.Files.Services;

public class FileHostingPaths(Hosting.HostEnvironment environment, IHttpContextAccessor httpContextAccessor, PublicFileHostingPaths publicPaths)
{
    public Conesoft.Files.Directory[] GetRoots()
    {
        var paths = publicPaths.Paths.Select(Conesoft.Files.Directory.From).ToArray();

        if (httpContextAccessor?.HttpContext?.User.Identity?.Name is string name)
        {
            var personal = environment.Global.Storage / "Users" / name / Filename.From("conesoft-files", "json");

            if (personal.Exists && personal.Now.ReadFromJson<UserMappings>() is UserMappings mappings)
            {
                return [
                    .. paths,
                .. mappings.Roots?.Select(Conesoft.Files.Directory.From) ?? [],
                .. mappings.Mapped?.Select(entry => new MappedDirectory(entry.Key, Conesoft.Files.Directory.From(entry.Value))) ?? []
                ];
            }
        }
        return paths;
    }

    public string PathAt(string? path) => path ?? "";
    public Conesoft.Files.File? FileAt(string? path) => GetRoots().Select(p => (p / PathAt(path)).AsFile).NotNull().FirstOrDefault();
    public Conesoft.Files.File[] FilesAt(string? path) => GetRoots().Select(p => (p / PathAt(path)).AsFile).NotNull().ToArray();
    public Conesoft.Files.Entry? EntryAt(string? path) => GetRoots().Select(p => (p / PathAt(path))).NotNull().FirstOrDefault();
    public Conesoft.Files.Entry[] EntriesAt(string? path) => GetRoots().Select(p => (p / PathAt(path))).NotNull().ToArray();
    public Conesoft.Files.Directory? DirectoryAt(string? path) => GetRoots().Select(p => (p / PathAt(path)).AsDirectory).NotNull().FirstOrDefault();
    public Conesoft.Files.Directory[] DirectoriesAt(string? path) => GetRoots().Select(p => (p / PathAt(path)).AsDirectory).NotNull().ToArray();

    record UserMappings(Dictionary<string, string> Mapped, string[] Roots);

    public record MappedDirectory(string MappedName, Conesoft.Files.Directory Root, bool InSubDirectory = false) : Conesoft.Files.Directory(Root.Parent)
    {
        public override string Name => InSubDirectory ? base.Name : MappedName;
        public override IEnumerable<Conesoft.Files.File> Files => InSubDirectory ? base.Files : [];

        public override IEnumerable<Conesoft.Files.Directory> Directories => InSubDirectory ? base.Directories : [Root];

        public override Conesoft.Files.Directory? AsDirectory => this;

        protected override Conesoft.Files.Directory SubDirectory(string subdirectory) => string.IsNullOrEmpty(subdirectory) ? this : base.SubDirectory(subdirectory);
    }
};

public static class MultiDirectoryExtensions
{
    public static IEnumerable<Conesoft.Files.File> Files(this IEnumerable<Conesoft.Files.Directory> directories) => directories.SelectMany(d => d.Files);
    public static IEnumerable<Conesoft.Files.Directory> Directories(this IEnumerable<Conesoft.Files.Directory> directories) => directories.SelectMany(d => d.Directories);
    public static IEnumerable<Conesoft.Files.Entry> Entries(this IEnumerable<Conesoft.Files.Directory> directories) => directories.SelectMany(d => (Entry[])[..d.Files, ..d.Directories]);

    public static async Task<CancellationTokenSource> Live(this IEnumerable<Conesoft.Files.Directory> directories, Func<Task> onChange)
    {
        await onChange();
        var cts = new CancellationTokenSource();
        var _ = Task.WhenAll(directories.Select(root => Task.Run(async () =>
        {
            try
            {
                await foreach (var _ in root.Live(false, cts.Token).EndOnCancel(cts.Token))
                {
                    await onChange();
                }
            }
            catch (Exception e)
            {
                Log.Error("exception: {exception}", e);
            }
        })));
        return cts;
    }

    public static async Task<CancellationTokenSource> Live(this Conesoft.Files.File file, Func<Task> onChange)
    {
        await onChange();
        var cts = new CancellationTokenSource();
        var _ = Task.Run(async () =>
        {
            try
            {
                await foreach (var _ in file.Live(cts.Token).EndOnCancel(cts.Token))
                {
                    await onChange();
                }
            }
            catch (Exception e)
            {
                Log.Error("exception: {exception}", e);
            }
        });
        return cts;
    }   

    public static void Live(this IEnumerable<Conesoft.Files.Directory> directories, Action onChange, CancellationToken cancellationToken = default)
    {
        onChange();
        var _ = Task.WhenAll(directories.Select(root => Task.Run(async () =>
        {
            try
            {
                await foreach (var _ in root.Live(false, cancellationToken).EndOnCancel(cancellationToken))
                {
                    onChange();
                }
            }
            catch (Exception e)
            {
                Log.Error("exception: {exception}", e);
            }
        })));
    }
}