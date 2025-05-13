using Ardalis.GuardClauses;
using FastEndpoints;
using TagStudio.WebApi.Features.Authentication;
using TagStudio.WebApi.Infrastructure.Data;

namespace TagStudio.WebApi.Features.Entries;

public class UpdateEntryEndpoint(
    ApplicationDbContext dbContext, CurrentUser user,
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

        var userId = user.GetId();
        Guard.Against.NotFound(req.Id, entry);
        Guard.Against.Forbidden(entry.OwnerId, userId);

        entry.Name = req.Name;
        entry.Description = req.Description;

        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} updated entry {EntryId}", userId, entry.Id);

        await SendNoContentAsync(ct);
    }
}