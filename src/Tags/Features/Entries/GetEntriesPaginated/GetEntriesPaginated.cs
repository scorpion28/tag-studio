using FastEndpoints;
using TagStudio.Tags.Common;
using TagStudio.Tags.Common.Constants;
using TagStudio.Tags.Common.Models;
using TagStudio.Tags.Data;

namespace TagStudio.Tags.Features.Entries;

public class GetEntriesPaginatedEndpoint(TagsDbContext dbContext)
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