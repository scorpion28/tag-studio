using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using TagStudio.Shared.User;
using TagStudio.Tags.Common;
using TagStudio.Tags.Common.Models.Tags;
using TagStudio.Tags.Data;

namespace TagStudio.Tags.Features.Tags;

public class GetTagEndpoint(TagsDbContext dbContext, CurrentUser user)
    : EndpointWithoutRequest<TagDetailedDto>
{
    public override void Configure()
    {
        Get("/tags/{id:guid}");
        
        Summary(s => s.Summary = "Get Tag");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("Id");

        var tag = await dbContext.Tags
            .Include(t => t.Parents)
            .Include(t => t.Children)
            .AsSplitQuery()
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        Guard.Against.NotFound(id, tag);
        Guard.Against.Forbidden(tag.OwnerId, user.Id);

        await SendOkAsync(tag.ToTagDetailedDto(), ct);
    }
}