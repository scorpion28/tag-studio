using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TagStudio.Identity.Data;

public static class IdentityMigrationExtensions
{
    public static async Task MigrateIdentityDbAsync(this IServiceProvider provider)
    {
        await provider.GetRequiredService<UsersDbContext>().Database.MigrateAsync();
    }
}