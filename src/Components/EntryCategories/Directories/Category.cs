using Conesoft.Files;

namespace Conesoft.Website.Files.Components.EntryCategories.Directories;

public class Category : Base.Category
{
    public override string? Name => null;

    public override IEnumerable<Entry> OrganisedEntries => base.OrganisedEntries.DistinctBy(e => e.Name);

    public override void From(IEnumerable<Entry> entries)
    {
        this.entries = entries.Where(e => e.IsDirectory);
    }
}
