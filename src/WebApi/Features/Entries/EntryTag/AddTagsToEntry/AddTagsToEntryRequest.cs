using FastEndpoints;
using TagStudio.WebApi.Common.Models;

namespace TagStudio.WebApi.Features.Entries.EntryTag;

public class AddTagsToEntryRequest : RequestWithUserId
{
    [RouteParam]
    public Guid EntryId { get; init; }

    public List<Guid> TagIds { get; init; } = [];
}