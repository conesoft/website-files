record FileHostingPath(string Path)
{
    public Conesoft.Files.Directory Directory => Conesoft.Files.Directory.From(Path);
};