using Microsoft.Extensions.DependencyInjection;
using TagStudio.WebApi.Domain.Common;
using TagStudio.WebApi.Infrastructure.Data;

namespace TagStudio.WebApi.FunctionalTests;

[Collection(SharedCollection.CollectionName)]
public class TestsBase : IAsyncLifetime
{
    private readonly TagStudioFactory _appFactory;

    public TestsBase(TagStudioFactory appFactory)
    {
        _appFactory = appFactory;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _appFactory.ResetDbAsync();
    }

    /// <summary>
    ///  Helper method that allows to save new entities to the database
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    protected async Task SeedDbAsync<T>(params IEnumerable<T> entities) 
        where T : class
    {
        await using var scope = _appFactory.Services.CreateAsyncScope();
        await using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await db.Set<T>().AddRangeAsync(entities);

        await db.SaveChangesAsync();
    }
}