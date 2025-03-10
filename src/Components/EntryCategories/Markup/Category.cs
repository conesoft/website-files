using Conesoft.Files;

namespace Conesoft.Website.Files.Components.EntryCategories.Markup;

public class Category : Base.Category
{
    public override void From(IEnumerable<Entry> entries)
    {
        string[] extensions = ["html", "md"];
        this.entries = entries.Files().Where(f => extensions.Contains(f.Extension.ToLower()));
    }
}
