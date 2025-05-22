namespace TagStudio.Tags.Common.Models;

public class EntryBriefDto
{
    public Guid Id { get; init; }

    public required string Name { get; init; }
    
    public string? Description { get; init; }
}