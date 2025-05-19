using TagStudio.WebApi.Common.Models;

namespace TagStudio.WebApi.Features.Entries;

public class CreateEntryRequest : RequestWithUserId
{
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}