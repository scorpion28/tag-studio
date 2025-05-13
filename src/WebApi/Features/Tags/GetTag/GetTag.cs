using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using TagStudio.WebApi.Common;
using TagStudio.WebApi.Features.Authentication;
using TagStudio.WebApi.Infrastructure.Data;

namespace TagStudio.WebApi.Features.Tags;

public class GetTagEndpoint(ApplicationDbContext dbContext, CurrentUser user)
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
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        Guard.Against.NotFound(id, tag);
        Guard.Against.Forbidden(tag.OwnerId, user.GetId());

        await SendOkAsync(tag.ToTagDetailedDto(), ct);
    }
}