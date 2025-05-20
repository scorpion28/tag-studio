using System.ComponentModel.DataAnnotations;
using TagStudio.Tags.Domain.Common;

namespace TagStudio.Tags.Domain;

public class Tag : BaseAuditableEntity
{
    [MaxLength(100)]
    public required string Name { get; set; }

    public List<Tag> Parents { get; } = [];
    public List<Tag> Children { get; } = [];

    public List<Entry> Entries { get; } = [];
}