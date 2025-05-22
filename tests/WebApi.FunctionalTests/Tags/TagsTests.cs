using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using TagStudio.Tags.Common;
using TagStudio.Tags.Common.Models.Tags;
using TagStudio.Tags.Features.Tags;
using TagStudio.WebApi.Common;
using TagStudio.WebApi.FunctionalTests.Common;

namespace TagStudio.WebApi.FunctionalTests.Tags;

public class TagsTests : TestsBase, IAsyncLifetime
{
    private readonly TagStudioFactory _appFactory;
    private readonly HttpClient _httpClient;
    private readonly TestDataGenerator _dataGenerator;

    private readonly Guid _userId;

    public TagsTests(TagStudioFactory appFactory) : base(appFactory)
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
    public async Task CreateTag_ShouldReturnCreatedTag()
    {
        var request = new CreateTagRequest { Name = "Tag" };

        var response = await _httpClient.PostAsJsonAsync("/tags", request);
        var createdTag = await response.Content.ReadFromJsonAsync<TagBriefDto>();

        response.IsSuccessStatusCode.ShouldBeTrue();

        createdTag.ShouldNotBeNull();
        createdTag.Id.ShouldNotBe(Guid.Empty);
        createdTag.Name.ShouldBe(request.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null!)]
    public async Task CreateTag_ShouldReturnBadRequest_WhenNameIsWhitespaceOrMissing(string? tagName)
    {
        var request = new CreateTagRequest { Name = tagName! };

        var response = await _httpClient.PostAsJsonAsync("/tags", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        // Verify error message
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails.ShouldNotBeNull();
        problemDetails.Errors.ShouldContainKey("name");
        problemDetails.Errors["name"].ShouldContain("Name cannot be empty");
    }

    [Fact]
    public async Task CreateTag_ShouldCreateTagsWithParentTags_WhenParentIdsArePresent()
    {
        // Arrange
        var tags = _dataGenerator.GenerateTags(count: 2);
        var expectedParentTags = tags.Select(tag => tag.ToTagBriefDto()).ToArray();

        await SeedDbAsync(tags);

        // Act
        var request = new CreateTagRequest { Name = "Tag", ParentTagsIds = tags.Select(t => t.Id).ToList() };
        var response = await _httpClient.PostAsJsonAsync("/tags", request);

        // Verify successful result
        response.IsSuccessStatusCode.ShouldBeTrue();

        var createdTag = await response.Content.ReadFromJsonAsync<TagDetailedDto>();

        // Assert
        createdTag.ShouldNotBeNull();
        createdTag.Parents
            .ShouldBeEqualOrdered(expectedParentTags, x => x.Id);
    }

    [Fact]
    public async Task GetTag_ShouldReturnTagDetails()
    {
        var expectedTag = _dataGenerator.GenerateTag(_userId);
        await SeedDbAsync(expectedTag);

        var response = await _httpClient.GetAsync($"/tags/{expectedTag.Id}");
        var actualTag = await response.Content.ReadFromJsonAsync<TagDetailedDto>();

        response.IsSuccessStatusCode.ShouldBeTrue();
        actualTag.ShouldNotBeNull();
        actualTag.ShouldBeEquivalentTo(expectedTag.ToTagDetailedDto());
    }

    [Fact]
    public async Task GetTag_ShouldReturnNotFound_WhenTagDoesNotExist()
    {
        var tagId = Guid.NewGuid();

        var response = await _httpClient.GetAsync($"/tags/{tagId}");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTag_ShouldReturnNoContent_WhenTagExists()
    {
        // Arrange
        var tagToRemove = _dataGenerator.GenerateTag(_userId);
        await SeedDbAsync(tagToRemove);

        // Act
        var response = await _httpClient.DeleteAsync($"/tags/{tagToRemove.Id}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // Verify tag is actually deleted
        var getResponse = await _httpClient.GetAsync($"/tags/{tagToRemove.Id}");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTag_ShouldReturnNotFound_WhenTagDoesNotExist()
    {
        var tagId = Guid.NewGuid();

        var response = await _httpClient.DeleteAsync($"/tags/{tagId}");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}