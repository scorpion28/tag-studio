namespace TagStudio.WebApi.Features.Entries;

public class EntryBriefDto
{
    public Guid Id { get; init; }

    public required string Name { get; init; }
}