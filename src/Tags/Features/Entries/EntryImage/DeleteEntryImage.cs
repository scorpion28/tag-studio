using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using TagStudio.Shared.User;
using TagStudio.Tags.Domain;
using TagStudio.Tags.Infrastructure.Blob;

namespace TagStudio.Tags.Features.Entries.EntryImage;

public record DeleteEntryImageRequest([property: RouteParam] Guid EntryId);

public class DeleteEntryImageEndpoint(IEntryRepository repository, IBlobService blobService, CurrentUser user)
    : Endpoint<DeleteEntryImageRequest, NoContent>
{
    public override void Configure()
    {
        Delete("/entries/{entryId:guid}/image");

        Summary(s => s.Summary = "Remove image from Entry");
    }

    public override async Task<NoContent> ExecuteAsync(DeleteEntryImageRequest req, CancellationToken ct)
    {
        var entry = await repository.GetByIdAsync(req.EntryId, ct);

        Guard.Against.NotFound(req.EntryId, entry);
        Guard.Against.Forbidden(user.Id, entry.OwnerId);

        if (entry.ImageFileName is not null)
        {
            await blobService.DeleteFileAsync(entry.ImageFileName);

            entry.ImageFileName = null;

            await repository.UnitOfWork.SaveChangesAsync(ct);
        }

        return TypedResults.NoContent();
    }
}