using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TagStudio.Shared.User;
using TagStudio.Tags.Domain;
using TagStudio.Tags.Domain.Exceptions;
using TagStudio.Tags.Features.Entries.EntryImage;
using TagStudio.Tags.Infrastructure.Blob;

namespace TagStudio.Tags.UnitTests.Features.Entries.EntryImage;

public class AddEntryImageTests
{
    private readonly IEntryRepository _repository = Substitute.For<IEntryRepository>();
    private readonly IBlobService _blobService = Substitute.For<IBlobService>();
    private readonly CurrentUser _user = Substitute.For<CurrentUser>();
    private readonly ILogger<AddEntryImageEndpoint> _logger = Substitute.For<ILogger<AddEntryImageEndpoint>>();

    private readonly AddEntryImageEndpoint _endpoint;
    private readonly IFormFile _image = Substitute.For<IFormFile>();

    public AddEntryImageTests()
    {
        _endpoint = Factory.Create<AddEntryImageEndpoint>(_repository, _blobService, _user, _logger);
    }

    [Fact]
    public async Task AddEntryImage_ShouldAddImage_WhenEntryExists()
    {
        // Arrange
        var entry = new Entry("Entry");

        _repository.GetByIdAsync(Arg.Any<Guid>()).Returns(entry);
        _blobService.UploadFileAsync(Arg.Any<IFormFile>()).Returns("image.jpg");

        var request = new AddEntryImageRequest(Guid.NewGuid(), _image);

        // Act
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        entry.ImageFileName.ShouldBe("image.jpg");
        await _repository.UnitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task AddEntryImage_ShouldDeleteOldImage_WhenEntryHasImage()
    {
        // Arrange
        var entry = new Entry("Entry") { ImageFileName = "old.jpg" };

        _repository.GetByIdAsync(Arg.Any<Guid>()).Returns(entry);
        _blobService.UploadFileAsync(Arg.Any<IFormFile>()).Returns("new.jpg");

        var request = new AddEntryImageRequest(Guid.NewGuid(), _image);

        // Act
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        entry.ImageFileName.ShouldBe("new.jpg");
        await _blobService.Received(1).DeleteFileAsync("old.jpg");
        await _repository.UnitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task AddEntryImage_ShouldThrowNotFoundException_WhenEntryDoesNotExist()
    {
        // Arrange
        _repository.GetByIdAsync(Arg.Any<Guid>()).Returns((Entry?)null);

        var request = new AddEntryImageRequest(Guid.NewGuid(), _image);

        // Act
        var executeAction = async () => await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await executeAction.ShouldThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AddEntryImage_ShouldThrowForbiddenAccessException_WhenUserIdDoesNotMatchEntryOwnerId()
    {
        // Arrange
        var entryId = Guid.NewGuid();
        var entryOwnerId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var entry = new Entry("Entry") { Id = entryId, OwnerId = entryOwnerId };

        _user.Id = userId;
        _repository.GetByIdAsync(entryId).Returns(entry);

        var request = new AddEntryImageRequest(entryId, _image);

        // Act
        var executeAction = async () => await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await executeAction.ShouldThrowAsync<ForbiddenAccessException>();
    }
}