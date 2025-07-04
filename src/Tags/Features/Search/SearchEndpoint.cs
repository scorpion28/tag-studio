using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using TagStudio.Search.Contracts;
using TagStudio.Tags.Data;

namespace TagStudio.Tags.Features.Search;

public class SearchEndpoint(TagsDbContext context, IEntrySearchService searchService)
    : Endpoint<SearchRequest, List<SearchResultItem>>
{
    public override void Configure()
    {
        Get("/search");
    }

    public override async Task<List<SearchResultItem>> ExecuteAsync(SearchRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Query))
        {
            return [];
        }

        var results = await searchService.SearchEntriesAsync(req.UserId, req.Query);

        return results
            .Select(document =>
                new SearchResultItem(
                    document.Id,
                    document.Title,
                    document.Description)
            )
            .ToList();
    }
}