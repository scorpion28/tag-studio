namespace TagStudio.Shared.Contracts;

public class EntryDeleted
{
    public required Guid Id { get; init; }
    
    public string? ImageFileName { get; init; }
}