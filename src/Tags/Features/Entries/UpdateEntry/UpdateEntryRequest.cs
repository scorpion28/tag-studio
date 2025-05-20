using FastEndpoints;

namespace TagStudio.Tags.Features.Entries;

public class UpdateEntryRequest
{
    [RouteParam]
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}