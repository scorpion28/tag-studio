using TagStudio.Tags.Common.Models.Tags;

namespace TagStudio.Tags.Common.Models;

public class EntryDetailedDto
{
    public Guid Id { get; init; }

    public required string Name { get; set; }
    public string? Description { get; set; }
    
    public List<TagBriefDto>? Tags { get; set; }

    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastModified { get; set; }
}