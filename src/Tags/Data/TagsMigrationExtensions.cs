using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TagStudio.Tags.Data;

public static class TagsMigrationExtensions
{
    public static async Task MigrateTagsDbAsync(this IServiceProvider provider)
    {
        await provider.GetRequiredService<TagsDbContext>().Database.MigrateAsync();
    }
}