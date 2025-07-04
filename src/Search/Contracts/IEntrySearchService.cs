using TagStudio.Search.Models;

namespace TagStudio.Search.Contracts;

public interface IEntrySearchService
{
    Task IndexEntryAsync(EntrySearchDocument entry);
    Task DeleteEntryAsync(Guid entryId);
    Task<IReadOnlyCollection<EntrySearchDocument>> SearchEntriesAsync(Guid userId, string searchTerm);
}