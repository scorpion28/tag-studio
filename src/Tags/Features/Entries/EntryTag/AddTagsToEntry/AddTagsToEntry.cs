using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TagStudio.Tags.Data;

namespace TagStudio.Tags.Features.Entries.EntryTag;

public class AddTagsToEntryEndpoint(TagsDbContext dbContext) : Endpoint<AddTagsToEntryRequest>
{
    public override void Configure()
    {
        Post("/entries/{entryId:guid}/tags");

        Summary(s => s.Summary = "Add Tags to Entry");
    }

    public override async Task HandleAsync(AddTagsToEntryRequest req, CancellationToken ct)
    {
        var entry = await dbContext.Entries.FindAsync([req.EntryId], ct);

        Guard.Against.NotFound(req.EntryId, entry);
        Guard.Against.Forbidden(entry.OwnerId, req.UserId);

        var validTags = await dbContext.Tags
            .Where(x => req.TagIds.Contains(x.Id))
            .ToListAsync(cancellationToken: ct);

        if (validTags.Count != req.TagIds.Count)
        {
            await SendResultAsync(TypedResults.BadRequest("One or more of provided tag ids are invalid"));
            return;
        }

        var uniqueTags = validTags.Except(entry.Tags);

        entry.Tags.AddRange(uniqueTags);
        await dbContext.SaveChangesAsync(ct);

        await SendNoContentAsync(ct);
    }
}