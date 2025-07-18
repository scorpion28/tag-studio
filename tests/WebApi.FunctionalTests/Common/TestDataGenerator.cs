﻿using Bogus;
using TagStudio.Tags.Domain;
using TagStudio.Tags.Domain.Common;

namespace TagStudio.WebApi.FunctionalTests.Common;

/// A utility class for generating test data using the Faker library.
/// It provides methods to generate single or multiple instances of <see cref="Tag"/> and <see cref="Entry"/>
public class TestDataGenerator
{
    private readonly Faker<Tag> _tagFaker;

    private readonly Faker<Entry> _entryFaker;

    private TestDataGenerator(Faker<Tag> tagFaker, Faker<Entry> entryFaker)
    {
        _tagFaker = tagFaker;
        _entryFaker = entryFaker;
    }

    /// <summary>
    /// Creates a new instance of <see cref="TestDataGenerator"/>, optionally configuring the generated entities with a specific owner ID.
    /// </summary>
    /// <param name="userId">The optional id to assign as the <see cref="BaseEntity.OwnerId"/> for all entities generated by this instance. </param>
    public static TestDataGenerator Create(Guid? userId = null)
    {
        var entryFaker = BuildEntryFaker();
        var tagFaker = BuildTagFaker();

        if (userId.HasValue)
        {
            entryFaker.WithOwnerId(userId.Value);
            tagFaker.WithOwnerId(userId.Value);
        }

        return new TestDataGenerator(tagFaker, entryFaker);
    }

    private static Faker<Entry> BuildEntryFaker()
    {
        return new Faker<Entry>()
            .CustomInstantiator(f => new Entry(name: f.Random.Word()))
            .RuleFor(x => x.Name, f => f.Lorem.Word())
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.Description, f => f.Lorem.Sentence());
    }

    private static Faker<Tag> BuildTagFaker()
    {
        return new Faker<Tag>()
            .RuleFor(x => x.Name, f => f.Lorem.Word())
            .RuleFor(x => x.Id, f => f.Random.Guid());
    }
    
    public Tag GenerateTag(Guid? ownerId = null)
    {
        if (ownerId.HasValue)
        {
            _tagFaker.WithOwnerId(ownerId.Value);
        }

        return _tagFaker.Generate();
    }

    public List<Tag> GenerateTags(int count = 5, Guid? ownerId = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count, nameof(count));

        if (ownerId.HasValue)
        {
            _tagFaker.WithOwnerId(ownerId.Value);
        }

        return _tagFaker.Generate(count);
    }

    public Entry GenerateEntry(Guid? ownerId = null)
    {
        if (ownerId.HasValue)
        {
            _entryFaker.WithOwnerId(ownerId.Value);
        }

        return _entryFaker.Generate();
    }

    public List<Entry> GenerateEntries(int count = 5, Guid? ownerId = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count, nameof(count));

        if (ownerId.HasValue)
        {
            _entryFaker.WithOwnerId(ownerId.Value);
        }

        return _entryFaker.Generate(count);
    }
}

file static class TestDataGeneratorExtensions
{
    /// <summary>
    /// Configures the Faker for <see cref="BaseEntity"/> to initialize <see cref="BaseEntity.OwnerId"/> with specific value
    /// </summary>
    public static Faker<T> WithOwnerId<T>(this Faker<T> faker, Guid ownerId)
        where T : BaseEntity
    {
        return faker.RuleFor(x => x.OwnerId, _ => ownerId);
    }
}