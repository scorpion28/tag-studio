using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TagStudio.Tags.Data;

namespace TagStudio.Tags.Features.Entries.EntryTag;

public class RemoveEntryTagEndpoint(TagsDbContext dbContext, ILogger<RemoveEntryTagEndpoint> logger)
    : Endpoint<RemoveEntryTagsRequest>
{
    public override void Configure()
    {
        Delete("/entries/{entryId:guid}/tags/{tagId:guid}");

        Summary(s => s.Summary = "Remove Tags from Entry");
    }

    public override async Task HandleAsync(RemoveEntryTagsRequest req, CancellationToken ct)
    {
        var entry = await dbContext.Entries.Include(e => e.Tags)
            .FirstOrDefaultAsync(e => e.Id == req.EntryId, ct);

        Guard.Against.NotFound(req.EntryId, entry);
        Guard.Against.Forbidden(entry.OwnerId, req.UserId);

        var tag = entry.Tags.FirstOrDefault(t => t.Id == req.TagId);
        if (tag is null)
        {
            logger.LogInformation("Entry {EntryId} does not contain tag {TagId} so it can't be deleted",
                entry.Id, req.TagId);
            
            await SendNotFoundAsync(ct);
            return;
        }

        entry.Tags.Remove(tag);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} removed tag {TagId} from entry {EntryId}", req.UserId, tag.Id, entry.Id);

        await SendNoContentAsync(ct);
    }
}