using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TagStudio.WebApi.Common;
using TagStudio.WebApi.Features.Authentication;
using TagStudio.WebApi.Infrastructure.Data;
using TagStudio.WebApi.Infrastructure.Data.Interceptors;
using TagStudio.WebApi.Infrastructure.Identity;

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

        // Error handling
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddEndpointsApiExplorer();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var config = builder.Configuration;

        // Database
        services.AddDatabaseServices(config);

        // Identity 
        services.AddIdentityCore<AppUser>()
            .AddSignInManager<SignInManager<AppUser>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        // Auth
        services.AddAuthenticationServices(config);

        return services;
    }

    private static void AddDatabaseServices(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlServer(config.GetConnectionString("database"));
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ApplicationDbContextInitializer>();
    }

    private static void AddAuthenticationServices(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddAppAuthentication(config);
        services.AddAuthorizationBuilder()
            .AddCurrentUserHandler();
        services.AddCurrentUser();
    }
}