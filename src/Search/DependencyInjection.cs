using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TagStudio.Search.Contracts;
using TagStudio.Search.Infrastructure;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddSearchServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var config = builder.Configuration;

        var elasticConnectionString = config.GetConnectionString("elasticsearch")!;
        var settings = new ElasticsearchClientSettings(new Uri(elasticConnectionString))
            .DefaultIndex("user-entries");

        var client = new ElasticsearchClient(settings);

        services.AddSingleton(client);
        services.AddScoped<IEntrySearchService, ElasticSearchService>();

        return services;
    }
}