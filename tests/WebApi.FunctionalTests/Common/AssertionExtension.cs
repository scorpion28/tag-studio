using TagStudio.Tags.Common.Models.Tags;
using TagStudio.Tags.Features.Tags;

// ReSharper disable RedundantCast

// ReSharper disable once CheckNamespace
namespace Shouldly;

public static class AssertionExtensions
{
    /// <summary>
    ///  Assert that two <see cref="TagDetailedDto"/> should be equal
    /// </summary>
    /// <remarks>For <see cref="TagDetailedDto.Parents"/> and <see cref="TagDetailedDto.Children"/>
    /// collections comparison  </remarks>
    public static void ShouldBeEquivalentTo(this TagDetailedDto first, TagDetailedDto second)
    {
        // Cast to base class to skip collections comparison
        (first as TagBriefDto).ShouldBeEquivalentTo(second as TagBriefDto);

        // Compare collections 
        first.Parents.ShouldBeEqualOrdered(second.Parents, x => x.Id);
        first.Children.ShouldBeEqualOrdered(second.Children, x => x.Id);
    }

    /// <summary>
    /// Assert that two <see cref="IEnumerable{T}"/> sequences should be equal
    /// when ordered by a specified key.
    /// </summary>
    public static void ShouldBeEqualOrdered<T, TKey>(this IEnumerable<T>? actual, IEnumerable<T>? expected,
        Func<T, TKey> keySelector)
    {
        if (actual is null && expected is null) return;
        Assert.False(actual is null != expected is null, "Only one of sequences is null");

        var firstOrdered = actual!.OrderBy(keySelector);
        var secondOrdered = expected!.OrderBy(keySelector);

        firstOrdered.ShouldBeEquivalentTo(secondOrdered);
    }
}