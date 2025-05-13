using FastEndpoints;
using TagStudio.WebApi.Infrastructure.Data;

namespace TagStudio.WebApi.Features.Entries;

public class CreateEntryEndpoint(ApplicationDbContext dbContext, ILogger<CreateEntryEndpoint> logger)
    : Endpoint<CreateEntryRequest, EntryDetailedDto, CreateEntryMapper>
{
    public override void Configure()
    {
        Post("/entries");
        
        Summary(s => s.Summary = "Create Entry");
    }

    public override async Task<EntryDetailedDto> ExecuteAsync(CreateEntryRequest req, CancellationToken ct)
    {
        var entity = Map.ToEntity(req);

        await dbContext.Entries.AddAsync(entity, ct);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} created entry {EntryId}", req.UserId, entity.Id);
        
        return Map.FromEntity(entity);
    }
}