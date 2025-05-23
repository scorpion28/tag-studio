using FastEndpoints;
using TagStudio.Tags.Common.Models;

namespace TagStudio.Tags.Features.Entries.EntryTag;

public class RemoveEntryTagsRequest : RequestWithUserId
{
    [RouteParam]
    public Guid EntryId { get; init; }

    [RouteParam]
    public Guid TagId { get; init; }
}