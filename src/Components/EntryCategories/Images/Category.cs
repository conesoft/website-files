using Conesoft.Files;

namespace Conesoft.Website.Files.Components.EntryCategories.Images;

public class Category : Base.Category
{
    public override void From(IEnumerable<Entry> entries)
    {
        string[] extensions = ["jpg", "jpeg", "png", "gif"];
        this.entries = entries.Files().Where(f => extensions.Contains(f.Extension[1..]));
    }
}
