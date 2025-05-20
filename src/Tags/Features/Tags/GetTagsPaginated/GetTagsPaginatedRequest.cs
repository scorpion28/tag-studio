using FastEndpoints;
using TagStudio.Tags.Common.Models;

namespace TagStudio.Tags.Features.Tags;

public class GetTagsPaginatedRequest : RequestWithUserId
{
    [QueryParam]
    public int? PageNumber { get; init; }

    [QueryParam]
    public int? PageSize { get; init; }
}