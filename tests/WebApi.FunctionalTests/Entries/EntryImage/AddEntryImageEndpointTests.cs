using System.Net;
using System.Net.Http.Json;
using TagStudio.Tags.Common.Models;
using TagStudio.WebApi.FunctionalTests.Common;

namespace TagStudio.WebApi.FunctionalTests.Entries.EntryImage;

public class AddEntryImageEndpointTests : TestsBase, IAsyncLifetime
{
    private readonly TagStudioFactory _appFactory;
    private readonly HttpClient _httpClient;
    private readonly TestDataGenerator _dataGenerator;
    private readonly Guid _userId;

    public AddEntryImageEndpointTests(TagStudioFactory appFactory) : base(appFactory)
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
    public async Task AddEntryImage_ShouldReturnOk_WhenEntryExists()
    {
        // Arrange
        var entry = _dataGenerator.GenerateEntry();

        await SeedDbAsync(entry);

        using var content = MultipartFormDataHelper.Construct("test-assets/image.jpg");

        var response = await _httpClient.PostAsync($"/entries/{entry.Id}/image", content);

        // Assert 
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var getEntryResponse = await _httpClient.GetAsync($"/entries/{entry.Id}");
        var createdEntry = await getEntryResponse.Content.ReadFromJsonAsync<EntryDetailedDto>();

        // Verify the image was actually added
        createdEntry.ShouldNotBeNull();
        createdEntry.ImageUrl.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task AddEntryImage_ShouldReturnOkAndReplaceOldImage_WhenEntryHasImage()
    {
        // Arrange
        var entry = _dataGenerator.GenerateEntry();
        await SeedDbAsync(entry);
        using var content = MultipartFormDataHelper.Construct("test-assets/image.jpg");

        // Populate entry with image
        var addImageFirstResponse = await _httpClient.PostAsync($"/entries/{entry.Id}/image", content);
        addImageFirstResponse.EnsureSuccessStatusCode();

        // Act - Add another image when the entry already has one
        var addImageSecondResponse = await _httpClient.PostAsync($"/entries/{entry.Id}/image", content);

        // Assert 
        addImageSecondResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var getEntryResponse = await _httpClient.GetAsync($"/entries/{entry.Id}");
        var createdEntry = await getEntryResponse.Content.ReadFromJsonAsync<EntryDetailedDto>();

        // Verify the image was actually added
        createdEntry.ShouldNotBeNull();
        createdEntry.ImageUrl.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task AddEntryImage_ShouldReturnNotFound_WhenEntryDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();

        using var content = MultipartFormDataHelper.Construct("test-assets/image.jpg");

        var response = await _httpClient.PostAsync($"/entries/{nonExistentId}/image", content);
        
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}