using TagStudio.Tags.Common.Models;

namespace TagStudio.Tags.Features.Entries;

public class GetEntriesPaginatedRequest : RequestWithUserId
{
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}