using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.Extensions.Logging;
using TagStudio.Tags.Data;

namespace TagStudio.Tags.Features.Tags;

public class UpdateTagEndpoint(TagsDbContext dbContext, ILogger<UpdateTagEndpoint> logger) : Endpoint<UpdateTagRequest>
{
    public override void Configure()
    {
        Patch("/tags/{id:guid}");

        Summary(s => s.Summary = "Update Tag");
    }

    public override async Task HandleAsync(UpdateTagRequest req, CancellationToken ct)
    {
        var entity = await dbContext.Tags.FindAsync([req.Id], ct);

        Guard.Against.NotFound(req.Id, entity);
        Guard.Against.Forbidden(entity.OwnerId, req.UserId);

        entity.Name = req.Name;

        var requestHasParentTags = req.ParentTagsIds is not null && req.ParentTagsIds.Count > 0;
        if (requestHasParentTags)
        {
            var parentTags = dbContext.Tags
                .Where(t => t.OwnerId == req.UserId)
                .Where(t => req.ParentTagsIds!.Contains(t.Id));

            entity.Parents.Clear();
            entity.Parents.AddRange(parentTags);
        }

        await dbContext.SaveChangesAsync(ct);
        
        logger.LogInformation("User {UserId} updated tag {TagId}", req.UserId, req.Id);
        
        await SendNoContentAsync(ct);
    }
}