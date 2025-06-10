using System.Net.Http.Headers;

namespace TagStudio.WebApi.FunctionalTests.Entries.EntryImage;

public static class MultipartFormDataHelper
{
    public static MultipartFormDataContent Construct(string filePath)
    {
        MultipartFormDataContent? content = null;
        try
        {
            var imageStreamContent = new StreamContent(File.OpenRead(filePath));

            imageStreamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpg");
            
            content = new MultipartFormDataContent();

            content.Add(imageStreamContent, "file", "test-image.jpg");

            return content;
        }
        catch
        {
            content?.Dispose();
            throw;
        }
    }

}