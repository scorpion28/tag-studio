using FastEndpoints;
using TagStudio.Tags.Common;
using TagStudio.Tags.Common.Constants;
using TagStudio.Tags.Common.Models;
using TagStudio.Tags.Common.Models.Tags;
using TagStudio.Tags.Data;

namespace TagStudio.Tags.Features.Tags;

public class GetTagsPaginated(TagsDbContext dbContext)
    : Endpoint<GetTagsPaginatedRequest, PaginatedList<TagDetailedDto>>
{
    public override void Configure()
    {
        Get("/tags");

        Summary(s => s.Summary = "Get Tags");
    }

    public override async Task<PaginatedList<TagDetailedDto>> ExecuteAsync(GetTagsPaginatedRequest req,
        CancellationToken ct)
    {
        return await dbContext.Tags
            .Where(x => x.OwnerId == req.UserId)
            .OrderByDescending(x => x.LastModified)
            .ProjectToTagDetailedDto()
            .PaginatedListAsync(req.PageNumber ?? 1, req.PageSize ?? RequestConstants.DefaultItemsPerPage);
    }
}