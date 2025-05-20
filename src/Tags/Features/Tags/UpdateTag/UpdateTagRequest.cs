using FastEndpoints;
using TagStudio.Tags.Common.Models;

namespace TagStudio.Tags.Features.Tags;

public class UpdateTagRequest : RequestWithUserId
{
    [RouteParam]
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public List<Guid>? ParentTagsIds { get; init; }
}