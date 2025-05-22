namespace TagStudio.Tags.Domain;

/// <summary>
/// Represent child-parent relationship between tags
/// </summary>
public class TagParent
{
    public required Guid ParentId { get; init; }
    public Tag Parent { get; init; } = null!;

    public required Guid ChildId { get; init; }
    public Tag Child { get; init; } = null!;

    public DateTimeOffset Created { get; } = DateTimeOffset.Now;
}