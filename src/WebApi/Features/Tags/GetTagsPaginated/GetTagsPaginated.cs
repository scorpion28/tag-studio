using FastEndpoints;
using TagStudio.WebApi.Common;
using TagStudio.WebApi.Common.Constants;
using TagStudio.WebApi.Common.Models;
using TagStudio.WebApi.Infrastructure.Data;

namespace TagStudio.WebApi.Features.Tags;

public class GetTagsPaginated(ApplicationDbContext dbContext)
    : Endpoint<GetTagsPaginatedRequest, PaginatedList<TagBriefDto>>
{
    public override void Configure()
    {
        Get("/tags");

        Summary(s => s.Summary = "Get Tags");
    }

    public override async Task<PaginatedList<TagBriefDto>> ExecuteAsync(GetTagsPaginatedRequest req,
        CancellationToken ct)
    {
        return await dbContext.Tags
            .Where(x => x.OwnerId == req.UserId)
            .OrderBy(x => x.Name)
            .ProjectToTagBriefDto()
            .PaginatedListAsync(req.PageNumber ?? 1, req.PageSize ?? RequestConstants.DefaultItemsPerPage);
    }
}