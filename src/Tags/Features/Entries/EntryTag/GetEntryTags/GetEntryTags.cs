using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using TagStudio.Shared.User;
using TagStudio.Tags.Common;
using TagStudio.Tags.Common.Models.Tags;
using TagStudio.Tags.Data;
using TagStudio.Tags.Features.Tags;

namespace TagStudio.Tags.Features.Entries.EntryTag;

public class GetEntryTagsEndpoint(TagsDbContext dbContext, CurrentUser user)
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
        Guard.Against.Forbidden(entry.OwnerId, user.Id);

        return entry.Tags.ToTagBriefDtoList();
    }
}