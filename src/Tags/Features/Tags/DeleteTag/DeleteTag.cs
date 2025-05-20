using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.Extensions.Logging;
using TagStudio.Shared.User;
using TagStudio.Tags.Data;

namespace TagStudio.Tags.Features.Tags;

public class DeleteTagEndpoint(
    TagsDbContext dbContext,
    CurrentUser user,
    ILogger<DeleteTagEndpoint> logger) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/tags/{id:Guid}");

        Summary(s => s.Summary = "Delete Tag");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");

        var entity = await dbContext.Tags
            .FindAsync([id], ct);

        Guard.Against.NotFound(id, entity);
        Guard.Against.Forbidden(entity.OwnerId, user.Id);

        dbContext.Tags.Remove(entity);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} deleted tag  {TagId}", user.Id, id);

        await SendNoContentAsync(ct);
    }
}