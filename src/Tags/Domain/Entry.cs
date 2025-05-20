using System.ComponentModel.DataAnnotations;
using TagStudio.Tags.Domain.Common;

namespace TagStudio.Tags.Domain;

public class Entry : BaseAuditableEntity
{
    public Entry(string name)
    {
        Name = name;
    }

    private Entry() { }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }

    public List<Tag> Tags { get; init; } = [];
}