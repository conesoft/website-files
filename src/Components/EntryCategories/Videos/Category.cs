using Conesoft.Files;

namespace Conesoft.Website.Files.Components.EntryCategories.Videos;

public class Category : Base.Category
{
    public override void From(IEnumerable<Entry> entries)
    {
        string[] extensions = ["mp4", "mkv", "ogv"];
        this.entries = entries.Files().Where(f => extensions.Contains(f.Extension));
    }
}
