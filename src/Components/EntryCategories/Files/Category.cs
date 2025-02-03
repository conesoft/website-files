using Conesoft.Files;

namespace Conesoft.Website.Files.Components.EntryCategories.Files;

public class Category : Base.Category
{
    public override void From(IEnumerable<Entry> entries)
    {
        this.entries = entries.Where(e => e.IsFile);
    }
}
