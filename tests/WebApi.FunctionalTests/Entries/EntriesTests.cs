using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using TagStudio.Tags.Common;
using TagStudio.Tags.Common.Models;
using TagStudio.Tags.Common.Models.Tags;
using TagStudio.Tags.Features.Entries;
using TagStudio.Tags.Features.Tags;
using TagStudio.WebApi.Common;
using TagStudio.WebApi.FunctionalTests.Common;

namespace TagStudio.WebApi.FunctionalTests.Entries;

public class EntriesTests : TestsBase, IAsyncLifetime
{
    private readonly TagStudioFactory _appFactory;
    private readonly HttpClient _httpClient;
    private readonly TestDataGenerator _dataGenerator;
    private readonly Guid _userId;

    public EntriesTests(TagStudioFactory appFactory) : base(appFactory)
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
    public async Task CreateEntry_ShouldReturnCreatedEntry()
    {
        var entry = _dataGenerator.GenerateEntry();
        await SeedDbAsync(entry);
        var request = new CreateEntryRequest { Name = entry.Name, Description = entry.Description };

        var response = await _httpClient.PostAsJsonAsync("/entries", request);

        var createdEntry = await response.Content.ReadFromJsonAsync<TagBriefDto>();
        createdEntry.ShouldNotBeNull();
        createdEntry.Id.ShouldNotBe(Guid.Empty);
        createdEntry.Name.ShouldBe(request.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateEntry_ShouldReturnBadRequest_WhenNameIsInvalid(string? name)
    {
        var request = new CreateEntryRequest { Name = name! };

        var response = await _httpClient.PostAsJsonAsync("/entries", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        // Verify error message
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails.ShouldNotBeNull();
        problemDetails.Errors.ShouldContainKey("name");
        problemDetails.Errors["name"].ShouldContain("Name cannot be empty");
    }

    [Fact]
    public async Task GetEntry_ShouldReturnEntryDetails_WhenEntryExists()
    {
        var expectedEntry = _dataGenerator.GenerateEntry();
        await SeedDbAsync(expectedEntry);

        var getEntryResponse = await _httpClient.GetAsync($"/entries/{expectedEntry.Id}");
        var actualEntry = await getEntryResponse.Content.ReadFromJsonAsync<EntryDetailedDto>();

        getEntryResponse.IsSuccessStatusCode.ShouldBeTrue();

        actualEntry.ShouldNotBeNull();
        actualEntry.ShouldBeEquivalentTo(expectedEntry.ToEntryDetailedDto());
    }

    [Fact]
    public async Task GetEntry_ShouldReturnNotFound_WhenEntryDoesNotExist()
    {
        var entryId = Guid.NewGuid();

        var response = await _httpClient.GetAsync($"/entries/{entryId}");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateEntry_ShouldReturnNoContent_WhenValidRequest()
    {
        var initialEntry = _dataGenerator.GenerateEntry();
        await SeedDbAsync(initialEntry);

        var request = new { Name = "Updated", Description = "Updated description" };
        var response = await _httpClient.PatchAsJsonAsync($"/entries/{initialEntry.Id}", request);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateEntry_ShouldReturnNotFound_WhenEntryDoesNotExist()
    {
        var entryId = Guid.NewGuid();
        var request = new UpdateEntryRequest { Name = "Updated" };

        var response = await _httpClient.PatchAsJsonAsync($"/entries/{entryId}", request);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteEntry_ShouldReturnNoContent_WhenEntryExists()
    {
        // Arrange
        var tagToRemove = _dataGenerator.GenerateEntry();
        await SeedDbAsync(tagToRemove);

        // Act
        var response = await _httpClient.DeleteAsync($"/entries/{tagToRemove.Id}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // Verify entry is actually deleted
        var getResponse = await _httpClient.GetAsync($"/entries/{tagToRemove.Id}");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteEntry_ShouldReturnNotFound_WhenEntryDoesNotExist()
    {
        var entryId = Guid.NewGuid();

        var response = await _httpClient.DeleteAsync($"/entries/{entryId}");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}