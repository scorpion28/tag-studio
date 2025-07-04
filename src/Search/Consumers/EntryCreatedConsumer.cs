using MassTransit;
using TagStudio.Search.Contracts;
using TagStudio.Search.Models;
using TagStudio.Shared.Contracts;

namespace TagStudio.Search.Consumers;

public class EntryCreatedConsumer(IEntrySearchService searchService) : IConsumer<EntryCreated>
{
    public async Task Consume(ConsumeContext<EntryCreated> context)
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