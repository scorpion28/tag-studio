using FastEndpoints;
using TagStudio.WebApi.Common.Models;

namespace TagStudio.WebApi.Features.Tags;

public class GetTagsPaginatedRequest : RequestWithUserId
{
    [QueryParam]
    public int? PageNumber { get; init; }

    [QueryParam]
    public int? PageSize { get; init; }
}