using Conesoft.Files;

namespace Conesoft.Website.Files.Components.EntryCategories.Text;

public class Category : Base.Category
{
    public override void From(IEnumerable<Entry> entries)
    {
        string[] extensions = ["txt", "nfo", "cs", "csproj", "xml", "json", "xaml"];
        this.entries = entries.Files().Where(f => extensions.Contains(f.Extension));
    }
}
