using MassTransit;
using TagStudio.Search.Contracts;
using TagStudio.Search.Models;
using TagStudio.Shared.Contracts;

namespace TagStudio.Search.Consumers;

public class EntryUpdatedConsumer(IEntrySearchService searchService) : IConsumer<EntryUpdated>
{
    public async Task Consume(ConsumeContext<EntryUpdated> context)
    {
        var message = context.Message;

        var entryDocument = new EntrySearchDocument
        {
            Id = message.Id,
            Title = message.Title,
            Description = message.Description,
            UserId = message.UserId
        };

        await searchService.IndexEntryAsync(entryDocument);
    }
}