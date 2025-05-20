using TagStudio.Identity.Data;
using TagStudio.Tags.Data;

namespace TagStudio.WebApi.Infrastructure;

public static class DbMigrationExtensions
{
    public static async Task MigrateDatabasesAsync(this IHost builder)
    {
        await using var scope = builder.Services.CreateAsyncScope();
        var sp = scope.ServiceProvider;

        await sp.MigrateTagsDbAsync();
        await sp.MigrateIdentityDbAsync();
    }
}