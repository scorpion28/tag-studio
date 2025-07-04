using MassTransit;
using TagStudio.Search.Contracts;
using TagStudio.Shared.Contracts;

namespace TagStudio.Search.Consumers;

public class EntryDeletedConsumer(IEntrySearchService searchService) : IConsumer<EntryDeleted>
{
    public async Task Consume(ConsumeContext<EntryDeleted> context)
    {
        var message = context.Message;

        await searchService.DeleteEntryAsync(message.Id);
    }
}