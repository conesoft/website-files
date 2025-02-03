using Conesoft.Files;
using Conesoft.Hosting;
using Conesoft.Tools;

namespace Conesoft.Website.Files.Components.EntryCategories.Base;

public abstract class Category
{
    public enum CategoryItemType
    {
        Category,
        EntryViewer,
        ListEntry
    }

    static protected readonly IComparer<string> naturalSortComparer = new NaturalSortComparer();
    protected IEnumerable<Entry> entries = [];

    public IEnumerable<Entry> Entries => entries;
    public virtual IEnumerable<Entry> OrganisedEntries => entries.OrderBy(e => e.Name, naturalSortComparer);

    public virtual string Namespace => GetType()?.Namespace?.Split(".").Last() ?? throw new Exception("Namespace not found in Category");
    public virtual string? Name => Safe.Try(() => Namespace);

    public Type? GetItemType(CategoryItemType itemType) => GetType().Assembly.GetTypes().Where(t => t.Namespace == GetType().Namespace && t.Name == itemType.ToString()).FirstOrDefault();

    public abstract void From(IEnumerable<Entry> entries);
}
