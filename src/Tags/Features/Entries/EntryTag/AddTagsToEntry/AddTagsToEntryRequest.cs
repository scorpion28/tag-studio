using FastEndpoints;
using TagStudio.Tags.Common.Models;

namespace TagStudio.Tags.Features.Entries.EntryTag;

public class AddTagsToEntryRequest : RequestWithUserId
{
    [RouteParam]
    public Guid EntryId { get; init; }

    public List<Guid> TagIds { get; init; } = [];
}