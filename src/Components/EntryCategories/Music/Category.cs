using Conesoft.Files;

namespace Conesoft.Website.Files.Components.EntryCategories.Music;

public class Category : Base.Category
{
    public override void From(IEnumerable<Entry> entries)
    {
        string[] extensions = ["mp3", "m4a", "flac", "wav", "ogg"];
        this.entries = entries.Files().Where(f => extensions.Contains(f.Extension[1..]));
    }
}
