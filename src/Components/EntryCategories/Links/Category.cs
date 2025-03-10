using Conesoft.Files;

namespace Conesoft.Website.Files.Components.EntryCategories.Links;

public class Category : Base.Category
{
    public override void From(IEnumerable<Entry> entries)
    {
        string[] extensions = ["url"];
        this.entries = entries.Files().Where(f => extensions.Contains(f.Extension.ToLower()));
    }
}
