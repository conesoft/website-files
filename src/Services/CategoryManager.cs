using Conesoft.Website.Files.Components.EntryCategories.Base;

namespace Conesoft.Website.Files.Services;

public class CategoryManager
{
    readonly Dictionary<string, Type> categoryTypes = [];
    readonly Type fallback = typeof(Components.EntryCategories.Files.Category);

    public CategoryManager()
    {
        var types = GetType()
            .Assembly
            .GetTypes()
            .Where(t => t.IsAbstract == false)
            .Where(t => typeof(Category).IsAssignableFrom(t))
            .Where(t => t != fallback)
            .NotNull()
            ;

        foreach (var type in types)
        {
            if (type.Namespace?.Split(".").Last() is string name)
            {
                categoryTypes.Add(name, type);
            }
        }
    }

    /// <summary>
    /// returns all categories filled with entries, sorted so unnamed
    /// categories come first, and the fallback category comes last,
    /// containing the rest of the entries
    /// </summary>
    public IEnumerable<Category> Group(IEnumerable<Conesoft.Files.Entry> entries)
    {
        IEnumerable<Category> categories = categoryTypes
            .Select(c => Activator.CreateInstance(c.Value) as Category)
            .NotNull()
            .Select(c =>
            {
                c.From(entries);
                return c;
            })
            .OrderBy(c => c.Name != null)
            ;

        if (Activator.CreateInstance(fallback) is Category f)
        {
            f.From(entries.Except(categories.SelectMany(c => c.Entries)));
            categories = categories.Append(f);
        };

        return categories
            .Where(c => c.Entries.Any())
            ;
    }
}