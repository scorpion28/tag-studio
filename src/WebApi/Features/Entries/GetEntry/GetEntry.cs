using Ardalis.GuardClauses;
using FastEndpoints;
using TagStudio.WebApi.Common;
using TagStudio.WebApi.Features.Authentication;
using TagStudio.WebApi.Infrastructure.Data;

namespace TagStudio.WebApi.Features.Entries;

public class GetEntryEndpoint(ApplicationDbContext dbContext, CurrentUser user)
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

        var entry = await dbContext.Entries.FindAsync([id], ct);

        Guard.Against.NotFound(id, entry);
        Guard.Against.Forbidden(entry.OwnerId, user.GetId());

        await SendAsync(entry.ToEntryDetailedDto(), cancellation: ct);
    }
}