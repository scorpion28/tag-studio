using System.Net;
using System.Net.Http.Json;
using TagStudio.Tags.Common.Models.Tags;
using TagStudio.Tags.Features.Tags;
using TagStudio.WebApi.FunctionalTests.Common;

namespace TagStudio.WebApi.FunctionalTests.Tags;

public class UpdateTagTests : TestsBase, IAsyncLifetime
{
    private readonly TagStudioFactory _appFactory;
    private readonly HttpClient _httpClient;
    private readonly TestDataGenerator _dataGenerator;

    private readonly Guid _userId;

    public UpdateTagTests(TagStudioFactory appFactory) : base(appFactory)
    {
        _userId = Guid.NewGuid();

        _appFactory = appFactory;
        _httpClient = appFactory.CreateAuthorizedClient(_userId);
        _dataGenerator = TestDataGenerator.Create(_userId);
    }

    public new async Task InitializeAsync()
    {
        await _appFactory.CreateUserAsync(_userId);
    }

    [Fact]
    public async Task UpdateTag_ShouldReturnOk_WhenValidRequest()
    {
        var initialTagData = _dataGenerator.GenerateTag(_userId);
        await SeedDbAsync(initialTagData);
        var request = new { Name = "Updated" };

        var response = await _httpClient.PatchAsJsonAsync($"/tags/{initialTagData.Id}", request);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateTag_ShouldReturnNotFound_WhenTagDoesNotExist()
    {
        var tagId = Guid.NewGuid();
        var request = new UpdateTagRequest { Name = "Updated" };

        var response = await _httpClient.PatchAsJsonAsync($"/tags/{tagId}", request);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTag_ShouldUpdateLastModifiedTime_WhenValidRequest()
    {
        // Arrange
        var initialTag = _dataGenerator.GenerateTag(_userId);
        await SeedDbAsync(initialTag);
        
        var request = new UpdateTagRequest { Name = "Updated" };
        
        // Act
        var response = await _httpClient.PatchAsJsonAsync($"/tags/{initialTag.Id}", request);

        // Check if request was successful
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // Get updated tag
        var getUpdatedTagResponse = await _httpClient.GetAsync($"/tags/{initialTag.Id}");
        var updatedTag = await getUpdatedTagResponse.Content.ReadFromJsonAsync<TagDetailedDto>();

        // Assert
        updatedTag.ShouldNotBeNull();
        updatedTag.LastModified.ShouldBeGreaterThan(initialTag.LastModified);
    }
}