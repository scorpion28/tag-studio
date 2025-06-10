using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TagStudio.Tags.Data;
using TagStudio.Tags.Data.Interceptors;
using TagStudio.Tags.Infrastructure.Blob;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddTagsServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var config = builder.Configuration;

        // Database
        services.AddDbContext<TagsDbContext>((sp, options) =>
        {
            options.UseSqlServer(config.GetConnectionString("database"));
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        
        // Blob storage
        builder.AddAzureBlobServiceClient("blob");
        builder.Services.AddSingleton<IBlobService, BlobStorageService>();

        return services;
    }
}