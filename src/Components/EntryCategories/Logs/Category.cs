using Conesoft.Files;

namespace Conesoft.Website.Files.Components.EntryCategories.Logs;

public class Category : Base.Category
{
    public override IEnumerable<Entry> OrganisedEntries => base.OrganisedEntries
        .Select(ReadDateFromFilename)
        .Where(v => v.date > DateOnly.FromDateTime(DateTime.Now.AddDays(-7).Date))
        .OrderByDescending(OrderByDate)
        .Select(v => v.entry);
    public override void From(IEnumerable<Entry> entries)
    {
        this.entries = entries.Files().Where(f => f.Extension == "log");
    }

    private (Entry entry, DateOnly date) ReadDateFromFilename(Entry entry)
    {
        var datetext = entry.Name[^12..^4];
        DateOnly.TryParse($"{datetext[0..4]}-{datetext[4..6]}-{datetext[6..8]}", out var date);
        return (entry, date);
    }

    private DateOnly OrderByDate((Entry entry, DateOnly date) value)
    {
        return value.date;
    }
}
