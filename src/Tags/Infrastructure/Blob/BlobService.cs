using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TagStudio.Tags.Infrastructure.Blob;

public class BlobStorageService(BlobServiceClient blobServiceClient, ILogger<BlobStorageService> logger) : IBlobService
{
    private readonly BlobContainerClient _containerClient = blobServiceClient.GetBlobContainerClient("media");

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        await _containerClient.CreateIfNotExistsAsync();

        var extension = Path.GetExtension(file.FileName);
        
        var uniqueFileName = Guid.CreateVersion7() + extension;
        var blobClient = _containerClient.GetBlobClient(uniqueFileName);

        await blobClient.UploadAsync(file.OpenReadStream());

        logger.LogInformation("File {FileUri} uploaded successfully.", uniqueFileName);

        return uniqueFileName;
    }

    public string GetPublicUrl(string fileName)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);

        return blobClient.GenerateSasUri(
                BlobSasPermissions.Read,
                DateTimeOffset.UtcNow.AddHours(1))
            .ToString();
    }

    public async Task DeleteFileAsync(string fileName)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);

        await blobClient.DeleteAsync();
        
        logger.LogInformation("File {FileUri} deleted successfully.", fileName);
    }
}