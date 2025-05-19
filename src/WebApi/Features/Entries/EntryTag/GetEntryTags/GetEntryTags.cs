using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using TagStudio.WebApi.Common;
using TagStudio.WebApi.Features.Authentication;
using TagStudio.WebApi.Features.Tags;
using TagStudio.WebApi.Infrastructure.Data;

namespace TagStudio.WebApi.Features.Entries.EntryTag;

public class GetEntryTagsEndpoint(ApplicationDbContext dbContext, CurrentUser user)
    : EndpointWithoutRequest<List<TagBriefDto>>
{
    public override void Configure()
    {
        Get("/entries/{entryId:guid}/tags");
        
        Summary(s => s.Summary = "Get Tags of Entry");
    }

    public override async Task<List<TagBriefDto>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("entryId");

        var entry = await dbContext.Entries
            .Include(p => p.Tags)
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync(ct);

        Guard.Against.NotFound(id, entry);
        Guard.Against.Forbidden(entry.OwnerId, user.GetId());

        return entry.Tags.ToTagBriefDtoList();
    }
}