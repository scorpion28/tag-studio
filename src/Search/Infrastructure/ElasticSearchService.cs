using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Logging;
using TagStudio.Search.Contracts;
using TagStudio.Search.Models;

namespace TagStudio.Search.Infrastructure;

internal class ElasticSearchService(
    ElasticsearchClient client,
    ILogger<ElasticSearchService> logger
) : IEntrySearchService
{
    private const string IndexName = "user-entries";
    private static readonly string[] SearchFields = ["title^3", "description", "tags.title^2"];

    public async Task IndexEntryAsync(EntrySearchDocument entry)
    {
        // Index the document. The ID is the document ID, making updates idempotent.
        var response = await client.IndexAsync(entry, i => i
            .Index(IndexName)
            .Id(entry.Id.ToString())
        );

        if (!response.IsValidResponse)
        {
            logger.LogError("Failed to index entry {EntryId}: {Error}", entry.Id, response.DebugInformation);
            throw new Exception("Search indexing failed");
        }
    }

    public async Task DeleteEntryAsync(Guid entryId)
    {
        // First, ensure the document belongs to the user before deleting
        // A simple delete by ID query with a term filter achieves this safely
        var response = await client.DeleteByQueryAsync<EntrySearchDocument>(d => d
            .Indices(IndexName)
            .Query(q => q
                .Bool(b => b
                    .Must(m => m.Term(t => t.Field(fld => fld.Id).Value(entryId.ToString())))
                )
            )
        );

        if (!response.IsValidResponse)
        {
            logger.LogError("Failed to delete entry {EntryId}", entryId);
        }
    }

    public async Task<IReadOnlyCollection<EntrySearchDocument>> SearchEntriesAsync(Guid userId, string searchTerm)
    {
        var response = await client.SearchAsync<EntrySearchDocument>(s => s
            .Index(IndexName)
            .Query(q => q
                .Bool(b => b
                    .Filter(f => f
                        .Term(t => t
                            .Field(fld => fld.UserId.Suffix("keyword"))
                            .Value(userId.ToString()))
                    )
                    .Must(m => m
                        .MultiMatch(mm => mm
                                .Query(searchTerm)
                                .Fields(SearchFields)
                                .Fuzziness(new Fuzziness("AUTO"))
                        )
                    )
                )
            )
        );

        if (!response.IsValidResponse)
        {
            logger.LogError("Search failed for user {UserId}: {Error}", userId, response.DebugInformation);
            return [];
        }

        return response.Documents;
    }
}