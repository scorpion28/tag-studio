namespace TagStudio.Shared.Contracts;

public record EntryCreated(
    Guid Id,
    Guid UserId,
    string Title,
    string Description
);