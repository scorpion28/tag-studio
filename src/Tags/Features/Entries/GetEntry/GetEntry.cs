using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using TagStudio.Shared.User;
using TagStudio.Tags.Common.Mapping;
using TagStudio.Tags.Common.Models;
using TagStudio.Tags.Data;

namespace TagStudio.Tags.Features.Entries;

public class GetEntryEndpoint(TagsDbContext dbContext, CurrentUser user)
    : EndpointWithoutRequest<EntryDetailedDto>
{
    public override void Configure()
    {
        Get("/entries/{id:guid}");

        Summary(s => s.Summary = "Get Entry");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");

        var entry = await dbContext.Entries
            .Include(e => e.Tags)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

        Guard.Against.NotFound(id, entry);
        Guard.Against.Forbidden(entry.OwnerId, user.Id);

        await SendAsync(entry.ToEntryDetailedDto(), cancellation: ct);
    }
}