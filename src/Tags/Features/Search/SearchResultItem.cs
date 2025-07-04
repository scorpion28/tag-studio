namespace TagStudio.Tags.Features.Search;

public record SearchResultItem(
    Guid Id,
    string Title,
    string? Description
);