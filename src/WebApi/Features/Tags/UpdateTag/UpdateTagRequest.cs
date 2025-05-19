using FastEndpoints;
using TagStudio.WebApi.Common.Models;

namespace TagStudio.WebApi.Features.Tags;

public class UpdateTagRequest : RequestWithUserId
{
    [RouteParam]
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public List<Guid>? ParentTagsIds { get; init; }
}