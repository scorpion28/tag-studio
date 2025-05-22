using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
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

        var tagToDelete = await dbContext.Tags
            .FindAsync([id], ct);

        Guard.Against.NotFound(id, tagToDelete);
        Guard.Against.Forbidden(tagToDelete.OwnerId, user.Id);

        // SQL Server doesn't allow cascade delete for relationships that may produce cycles or multiple cascade paths
        // so the related relationships need to be deleted manually before the tag itself 
        await dbContext.TagParents.Where(parent => parent.ParentId == tagToDelete.Id).ExecuteDeleteAsync(ct);
        
        dbContext.Tags.Remove(tagToDelete);

        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} deleted tag  {TagId}", user.Id, id);

        await SendNoContentAsync(ct);
    }
}