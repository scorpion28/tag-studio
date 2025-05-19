using TagStudio.WebApi.Common.Models;

namespace TagStudio.WebApi.Features.Entries;

public class GetEntriesPaginatedRequest : RequestWithUserId
{
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}