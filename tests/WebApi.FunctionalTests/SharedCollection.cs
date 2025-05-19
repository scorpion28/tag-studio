namespace TagStudio.WebApi.FunctionalTests;

[CollectionDefinition(CollectionName)]
public class SharedCollection : ICollectionFixture<TagStudioFactory>
{
    public const string CollectionName = "TagStudioApi collection";
}