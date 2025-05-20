using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TagStudio.Identity.Data;
using TagStudio.Identity.Domain;
using TagStudio.Identity.Features;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddAppIdentityServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var config = builder.Configuration;

        services.AddDbContext<UsersDbContext>((sp, options) =>
        {
            options.UseSqlServer(config.GetConnectionString("database"));
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });
        
        services.AddIdentityCore<AppUser>()
            .AddSignInManager<SignInManager<AppUser>>()
            .AddEntityFrameworkStores<UsersDbContext>();

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.User.RequireUniqueEmail = true;

            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
        });

        services.AddScoped<ITokenService, JwtTokenService>();

        return services;
    }
}
