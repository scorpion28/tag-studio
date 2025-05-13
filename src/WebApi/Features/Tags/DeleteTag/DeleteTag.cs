using Ardalis.GuardClauses;
using FastEndpoints;
using TagStudio.WebApi.Features.Authentication;
using TagStudio.WebApi.Infrastructure.Data;

namespace TagStudio.WebApi.Features.Tags;

public class DeleteTagEndpoint(
    ApplicationDbContext dbContext,
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

        var userId = user.GetId();
        Guard.Against.NotFound(id, entity);
        Guard.Against.Forbidden(entity.OwnerId, userId);

        dbContext.Tags.Remove(entity);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} deleted tag  {TagId}", userId, id);

        await SendNoContentAsync(ct);
    }
}