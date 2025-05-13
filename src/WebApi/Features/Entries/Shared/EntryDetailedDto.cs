using TagStudio.WebApi.Domain;

namespace TagStudio.WebApi.Features.Entries;

public class EntryDetailedDto
{
    public Guid Id { get; init; }

    public required string Name { get; set; }
    public string? Description { get; set; }
    
    public List<Tag>? Tags { get; set; }

    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastModified { get; set; }
}