using Conesoft.Files;
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
    
    static private readonly Type baseType = typeof(Category);
    static private readonly Type[] types = baseType.Assembly.GetTypes();
    
    protected IEnumerable<Entry> entries = [];

    public IEnumerable<Entry> Entries => entries;
    public virtual IEnumerable<Entry> OrganisedEntries => entries.Visible().OrderBy(e => e.Name, naturalSortComparer);

    public virtual string Namespace => GetType()?.Namespace?.Split(".").Last() ?? throw new Exception("Namespace not found in Category");
    public virtual string? Name => Safe.Try(() => Namespace);

    public Type GetItemType(CategoryItemType itemType) =>
        types.Where(t => t.Namespace == GetType().Namespace && t.Name == itemType.ToString()).FirstOrDefault()
        ??
        types.Where(t => t.Namespace == baseType.Namespace && t.Name == itemType.ToString()).First()
        ;

    public abstract void From(IEnumerable<Entry> entries);
}
