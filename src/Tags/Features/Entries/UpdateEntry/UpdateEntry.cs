using Ardalis.GuardClauses;
using FastEndpoints;
using MassTransit;
using Microsoft.Extensions.Logging;
using TagStudio.Shared.Contracts;
using TagStudio.Shared.User;
using TagStudio.Tags.Data;

namespace TagStudio.Tags.Features.Entries;

public class UpdateEntryEndpoint(
    TagsDbContext dbContext,
    IPublishEndpoint publishEndpoint,
    CurrentUser user,
    ILogger<UpdateEntryEndpoint> logger) : Endpoint<UpdateEntryRequest>
{
    public override void Configure()
    {
        Patch("/entries/{id:guid}");

        Summary(s => s.Summary = "Update Entry");
    }

    public override async Task HandleAsync(UpdateEntryRequest req, CancellationToken ct)
    {
        var entry = await dbContext.Entries.FindAsync([req.Id], ct);

        Guard.Against.NotFound(req.Id, entry);
        Guard.Against.Forbidden(entry.OwnerId, user.Id);

        entry.Name = req.Name;
        entry.Description = req.Description;

        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} updated entry {EntryId}", user.Id, entry.Id);

        await publishEndpoint.Publish(new EntryUpdated(entry.Id, entry.OwnerId, entry.Name, entry.Description ?? ""));
        
        await SendNoContentAsync(ct);
    }
}