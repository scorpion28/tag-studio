using System.Net;
using System.Net.Http.Json;
using TagStudio.Tags.Common.Models;
using TagStudio.WebApi.FunctionalTests.Common;

namespace TagStudio.WebApi.FunctionalTests.Entries.EntryImage;

public class DeleteEntryImageEndpointTests : TestsBase, IAsyncLifetime
{
    private readonly TagStudioFactory _appFactory;
    private readonly HttpClient _httpClient;
    private readonly TestDataGenerator _dataGenerator;
    private readonly Guid _userId;


    public DeleteEntryImageEndpointTests(TagStudioFactory appFactory) : base(appFactory)
    {
        _appFactory = appFactory;

        _userId = Guid.NewGuid();
        _httpClient = appFactory.CreateAuthorizedClient(_userId);
        _dataGenerator = TestDataGenerator.Create(_userId);
    }

    public new async Task InitializeAsync()
    {
        await _appFactory.CreateUserAsync(_userId);
    }

    [Fact]
    public async Task DeleteEntryImage_ShouldReturnNoContent_WhenWhenValidRequest()
    {
        // Arrange
        var entry = _dataGenerator.GenerateEntry();

        await SeedDbAsync(entry);

        using var content = MultipartFormDataHelper.Construct("test-assets/image.jpg");

        // Add an image to the entry
        var addPhotoResponse = await _httpClient.PostAsync($"/entries/{entry.Id}/image", content);
        addPhotoResponse.EnsureSuccessStatusCode();

        // Act
        var deleteImageResponse = await _httpClient.DeleteAsync($"/entries/{entry.Id}/image");

        // Assert 
        deleteImageResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var getEntryResponse = await _httpClient.GetAsync($"/entries/{entry.Id}");
        var receivedEntry = await getEntryResponse.Content.ReadFromJsonAsync<EntryDetailedDto>();

        // Verify the image was actually removed
        receivedEntry.ShouldNotBeNull();
        receivedEntry.ImageUrl.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteEntryImage_ShouldReturnNoContent_WhenEntryHasNoImage()
    {
        var entry = _dataGenerator.GenerateEntry();
        await SeedDbAsync(entry);

        var response = await _httpClient.DeleteAsync($"/entries/{entry.Id}/image");
        
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task DeleteEntryImage_ShouldReturnNotFound_WhenEntryDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();

        var response = await _httpClient.DeleteAsync($"/entries/{nonExistentId}/image");
        
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

}