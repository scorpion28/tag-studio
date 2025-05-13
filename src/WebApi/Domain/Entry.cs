using TagStudio.WebApi.Domain.Common;

namespace TagStudio.WebApi.Domain;

public class Entry : BaseAuditableEntity
{
    public Entry(string name)
    {
        Name = name;
    }

    private Entry() { }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public List<Tag> Tags { get; init; } = [];
}