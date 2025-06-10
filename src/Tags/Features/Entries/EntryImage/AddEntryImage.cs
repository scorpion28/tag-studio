using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using TagStudio.Shared.User;
using TagStudio.Tags.Domain;
using TagStudio.Tags.Infrastructure.Blob;

namespace TagStudio.Tags.Features.Entries.EntryImage;

public record AddEntryImageRequest([property: RouteParam] Guid EntryId, IFormFile File);

public class AddEntryImageEndpoint(
    IEntryRepository repository,
    IBlobService blobService,
    CurrentUser user,
    ILogger<AddEntryImageEndpoint> logger
) : Endpoint<AddEntryImageRequest, Ok>
{
    public override void Configure()
    {
        Post("/entries/{entryId:guid}/image/");
        AllowFileUploads();

        Summary(s => s.Summary = "Add an image to Entry");
    }

    public override async Task<Ok> ExecuteAsync(AddEntryImageRequest req, CancellationToken ct)
    {
        var entry = await repository.GetByIdAsync(req.EntryId, ct);

        Guard.Against.NotFound(req.EntryId, entry);
        Guard.Against.Forbidden(entry.OwnerId, user.Id);

        var oldImage = entry.ImageFileName;
        var newImage = await blobService.UploadFileAsync(req.File);

        entry.ImageFileName = newImage;
        await repository.UnitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Image {NewImageName} added to entry {EntryId}", newImage, req.EntryId);

        if (oldImage is not null)
        {
            await blobService.DeleteFileAsync(oldImage);
        }

        return TypedResults.Ok();
    }
}