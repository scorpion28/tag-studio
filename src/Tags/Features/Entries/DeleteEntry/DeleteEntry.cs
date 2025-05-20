using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.Extensions.Logging;
using TagStudio.Shared.User;
using TagStudio.Tags.Data;

namespace TagStudio.Tags.Features.Entries;

public class DeleteEntryEndpoint(
    TagsDbContext dbContext,
    CurrentUser user,
    ILogger<DeleteEntryEndpoint> logger) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/entries/{id:guid}");

        Summary(s => s.Summary = "Delete Entry");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var entity = await dbContext.Entries
            .FindAsync([id], ct);

        Guard.Against.NotFound(id, entity);
        Guard.Against.Forbidden(entity.OwnerId, user.Id);

        dbContext.Entries.Remove(entity);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} deleted entry {EntryId}", user.Id, entity.Id);

        await SendNoContentAsync(ct);
    }
}