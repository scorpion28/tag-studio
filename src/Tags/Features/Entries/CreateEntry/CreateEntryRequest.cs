using TagStudio.Tags.Common.Models;

namespace TagStudio.Tags.Features.Entries;

public class CreateEntryRequest : RequestWithUserId
{
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}