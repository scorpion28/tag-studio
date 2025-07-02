using MassTransit;
using TagStudio.Shared.Contracts;
using TagStudio.Tags.Infrastructure.Blob;

namespace TagStudio.Tags.Consumers;

public class EntryDeletedConsumer(IBlobService blobService) : IConsumer<EntryDeleted>
{
    public async Task Consume(ConsumeContext<EntryDeleted> context)
    {
        var message = context.Message;

        if (message.ImageFileName is not null)
        {
            await blobService.DeleteFileAsync(message.ImageFileName);
        }
    }
}