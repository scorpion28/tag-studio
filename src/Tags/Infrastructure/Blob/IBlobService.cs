using Microsoft.AspNetCore.Http;

namespace TagStudio.Tags.Infrastructure.Blob;

public interface IBlobService
{
    Task<string> UploadFileAsync(IFormFile file);

    string GetPublicUrl(string fileName);

    Task DeleteFileAsync(string fileName);

}