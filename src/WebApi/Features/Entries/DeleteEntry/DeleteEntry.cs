using Ardalis.GuardClauses;
using FastEndpoints;
using TagStudio.WebApi.Features.Authentication;
using TagStudio.WebApi.Infrastructure.Data;

namespace TagStudio.WebApi.Features.Entries;

public class DeleteEntryEndpoint(
    ApplicationDbContext dbContext,
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

        var userId = user.GetId();
        Guard.Against.NotFound(id, entity);
        Guard.Against.Forbidden(entity.OwnerId, userId);

        dbContext.Entries.Remove(entity);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} deleted entry {EntryId}", userId, entity.Id);

        await SendNoContentAsync(ct);
    }
}