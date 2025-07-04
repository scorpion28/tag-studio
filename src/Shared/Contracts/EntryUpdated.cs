namespace TagStudio.Shared.Contracts;

public record EntryUpdated(
    Guid Id,
    Guid UserId,
    string Title,
    string Description
);