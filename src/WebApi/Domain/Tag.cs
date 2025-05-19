using TagStudio.WebApi.Domain.Common;

namespace TagStudio.WebApi.Domain;

public class Tag : BaseAuditableEntity
{
    public required string Name { get; set; }

    public List<Tag> Parents { get; } = [];
    public List<Tag> Children { get; } = [];

    public List<Entry> Entries { get; } = [];
}