using FastEndpoints;
using TagStudio.WebApi.Common;
using TagStudio.WebApi.Common.Constants;
using TagStudio.WebApi.Common.Models;
using TagStudio.WebApi.Infrastructure.Data;

namespace TagStudio.WebApi.Features.Entries;

public class GetEntriesPaginatedEndpoint(ApplicationDbContext dbContext)
    : Endpoint<GetEntriesPaginatedRequest, PaginatedList<EntryBriefDto>>
{
    public override void Configure()
    {
        Get("/entries");

        Summary(s => s.Summary = "Get Entries");
    }

    public override async Task<PaginatedList<EntryBriefDto>> ExecuteAsync(GetEntriesPaginatedRequest req,
        CancellationToken ct)
    {
        return await dbContext.Entries
            .Where(e => e.OwnerId == req.UserId)
            .OrderBy(p => p.Name)
            .ProjectToEntryBriefDto()
            .PaginatedListAsync(req.PageNumber ?? 1, req.PageSize ?? RequestConstants.MaxItemsPerPage);
    }
}