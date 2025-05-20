using System.Net;
using System.Net.Http.Json;
using TagStudio.Tags.Common;
using TagStudio.Tags.Common.Models.Tags;
using TagStudio.Tags.Features.Tags;
using TagStudio.WebApi.Common;
using TagStudio.WebApi.FunctionalTests.Common;

namespace TagStudio.WebApi.FunctionalTests.Entries.EntryTags;

public class EntryTagsTests : TestsBase, IAsyncLifetime
{
    private readonly TagStudioFactory _appFactory;
    private readonly HttpClient _httpClient;
    private readonly TestDataGenerator _dataGenerator;
    private readonly Guid _userId;

    public EntryTagsTests(TagStudioFactory appFactory) : base(appFactory)
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
    public async Task GetEntryTags_ShouldReturnAllTags()
    {
        // Arrange
        var entry = _dataGenerator.GenerateEntry();
        var tags = _dataGenerator.GenerateTags(count: 4);
        entry.Tags.AddRange(tags);
        await SeedDbAsync(entry);

        var expectedTags = tags.ToTagBriefDtoList();

        // Act
        var request = await _httpClient.GetAsync($"/entries/{entry.Id}/tags");

        // Verify response status
        request.IsSuccessStatusCode.ShouldBeTrue();

        // Assert
        var actualTags = await request.Content.ReadFromJsonAsync<List<TagBriefDto>>();
        actualTags.ShouldNotBeNull();
        actualTags.Count.ShouldBe(expectedTags.Count);

        actualTags.ShouldBeEqualOrdered(expectedTags, x => x.Id);
    }

    [Fact]
    public async Task AddTagToEntry_ShouldReturnNoContent_WhenTagsSuccessfullyAdded()
    {
        // Arrange
        var entry = _dataGenerator.GenerateEntry();
        var tagToAdd = _dataGenerator.GenerateTag();
        await SeedDbAsync(entry);
        await SeedDbAsync(tagToAdd);

        var request = new { TagIds = new[] { tagToAdd.Id } };

        // Act
        var response = await _httpClient.PostAsJsonAsync($"/entries/{entry.Id}/tags", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // Verify tag was actually added
        var getResponse = await _httpClient.GetAsync($"/entries/{entry.Id}/tags");
        getResponse.IsSuccessStatusCode.ShouldBeTrue();

        var expectedTag = tagToAdd.ToTagBriefDto();

        var entryTags = await getResponse.Content.ReadFromJsonAsync<List<TagBriefDto>>();
        entryTags.ShouldNotBeNull();
        entryTags.Single().ShouldBeEquivalentTo(expectedTag);
    }

    [Fact]
    public async Task AddTagToEntry_ShouldReturnBadRequest_WhenTagDoesNotExist()
    {
        var entry = _dataGenerator.GenerateEntry();
        await SeedDbAsync(entry);

        var nonExistingTagId = Guid.NewGuid();

        var request = new { TagIds = new[] { nonExistingTagId } };
        var response = await _httpClient.PostAsJsonAsync($"/entries/{entry.Id}/tags", request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}