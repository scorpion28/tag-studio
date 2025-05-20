using FastEndpoints;
using FastEndpoints.Swagger;
using TagStudio.WebApi.Common;
using TagStudio.WebApi.Common.Authentication;

// ReSharper disable once CheckNamespace
internal static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddFastEndpoints()
            .SwaggerDocument(o => o.ShortSchemaNames = true);

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policyBuilder =>
            {
                policyBuilder.AllowAnyOrigin();
                policyBuilder.AllowAnyMethod();
                policyBuilder.AllowAnyHeader();
            });
        });

        // Auth
        services.AddAuthenticationServices(builder.Configuration);


        // Error handling
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddEndpointsApiExplorer();

        return services;
    }

    private static void AddAuthenticationServices(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddAppAuthentication(config);
        services.AddAuthorizationBuilder()
            .AddCurrentUserHandler();
        services.AddCurrentUser();
    }
}