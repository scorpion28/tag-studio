using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using TagStudio.Tags.Common;
using TagStudio.Tags.Common.Constants;
using TagStudio.Tags.Common.Mapping;
using TagStudio.Tags.Common.Models;
using TagStudio.Tags.Data;
using TagStudio.Tags.Infrastructure.Blob;

namespace TagStudio.Tags.Features.Entries;

public class GetEntriesPaginatedEndpoint(TagsDbContext dbContext, IBlobService blobService)
    : Endpoint<GetEntriesPaginatedRequest, PaginatedList<EntryDetailedDto>>
{
    public override void Configure()
    {
        Get("/entries");

        Summary(s => s.Summary = "Get Entries");
    }

    public override async Task<PaginatedList<EntryDetailedDto>> ExecuteAsync(GetEntriesPaginatedRequest req,
        CancellationToken ct)
    {
        var page = await dbContext.Entries
            .Include(e => e.Tags)
            .Where(e => e.OwnerId == req.UserId)
            .OrderBy(p => p.Created)
            .ProjectToEntryDetailedDto()
            .PaginatedListAsync(req.PageNumber ?? 1, req.PageSize ?? RequestConstants.MaxItemsPerPage);

        // Enrich with public image URLs
        foreach (var item in page.Items)
        {
            if (!string.IsNullOrEmpty(item.ImageFileName))
            {
                item.ImageUrl = blobService.GetPublicUrl(item.ImageFileName);
            }
        }

        return page;
    }
}