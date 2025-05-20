using FastEndpoints;
using Microsoft.Extensions.Logging;
using TagStudio.Tags.Common.Models;
using TagStudio.Tags.Data;

namespace TagStudio.Tags.Features.Entries;

public class CreateEntryEndpoint(TagsDbContext dbContext, ILogger<CreateEntryEndpoint> logger)
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