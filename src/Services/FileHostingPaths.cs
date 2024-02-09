record FileHostingPaths(params string[] Paths)
{
    public Conesoft.Files.Directory[] Roots => Paths.Select(Conesoft.Files.Directory.From).ToArray();
};