using Ardalis.GuardClauses;
using FastEndpoints;
using TagStudio.Shared.User;
using TagStudio.Tags.Domain;
using TagStudio.Tags.Domain.Exceptions;
using TagStudio.Tags.Features.Entries.EntryImage;
using TagStudio.Tags.Infrastructure.Blob;

namespace TagStudio.Tags.UnitTests.Features.Entries.EntryImage;

public class DeleteEntryImageTests
{
    private readonly IEntryRepository _repository = Substitute.For<IEntryRepository>();
    private readonly IBlobService _blobService = Substitute.For<IBlobService>();
    private readonly CurrentUser _user = Substitute.For<CurrentUser>();

    private readonly DeleteEntryImageEndpoint _endpoint;

    public DeleteEntryImageTests()
    {
        _endpoint = Factory.Create<DeleteEntryImageEndpoint>(_repository, _blobService, _user);
    }

    [Fact]
    public async Task DeleteEntry_ShouldDeleteImage_WhenEntryHasImage()
    {
        // Arrange
        var entryId = Guid.NewGuid();
        var entry = new Entry("Entry") { Id = entryId, ImageFileName = "image.jpg" };

        _repository.GetByIdAsync(entryId).Returns(entry);

        var request = new DeleteEntryImageRequest(entryId);

        // Act
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        entry.ImageFileName.ShouldBeNull();
        await _blobService.Received(1).DeleteFileAsync("image.jpg");
        await _repository.UnitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task DeleteEntry_ShouldNotChangeEntryOrDeleteImages_WhenEntryHasNoImage()
    {
        // Arrange
        var entryId = Guid.NewGuid();

        var entry = new Entry("Entry");

        _repository.GetByIdAsync(entryId).Returns(entry);

        var request = new DeleteEntryImageRequest(entryId);

        // Act
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        entry.ImageFileName.ShouldBeNull();
        await _blobService.DidNotReceive().DeleteFileAsync(Arg.Any<string>());
        await _repository.UnitOfWork.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task DeleteEntryImage_ShouldThrowNotFoundException_WhenEntryDoesNotExist()
    {
        // Arrange
        _repository.GetByIdAsync(Arg.Any<Guid>()).Returns((Entry?)null);

        var request = new DeleteEntryImageRequest(Guid.NewGuid());

        // Act
        var executeAction = async () => await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await executeAction.ShouldThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteEntryImage_ShouldThrowForbiddenAccessException_WhenUserIdDoesNotMatchOwnerOfEntry()
    {
        // Arrange
        var entryId = Guid.NewGuid();
        var entryOwnerId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var entry = new Entry("Entry") { Id = entryId, OwnerId = entryOwnerId };

        _user.Id = userId;
        _repository.GetByIdAsync(entryId).Returns(entry);

        var request = new DeleteEntryImageRequest(entryId);
        
        // Act
        var executeAction = async () => await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await executeAction.ShouldThrowAsync<ForbiddenAccessException>();
    }
}