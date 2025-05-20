using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Respawn;
using TagStudio.Identity.Data;
using TagStudio.Identity.Domain;
using TagStudio.Identity.Features;
using TagStudio.Tags.Data;
using Testcontainers.MsSql;

namespace TagStudio.WebApi.FunctionalTests;

// ReSharper disable once ClassNeverInstantiated.Global
public class TagStudioFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder().Build();

    private Respawner _respawner = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddDbContext<TagsDbContext>(options =>
            {
                options.UseSqlServer(_dbContainer.GetConnectionString());
            });
            services.AddDbContext<UsersDbContext>(options =>
            {
                options.UseSqlServer(_dbContainer.GetConnectionString());
            });
        });

        builder.ConfigureLogging(logging => { logging.ClearProviders(); });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        await MigrateDatabasesAsync();

        _respawner = await Respawner.CreateAsync(_dbContainer.GetConnectionString(), new RespawnerOptions
        {
            TablesToIgnore = ["__EFMigrationsHistory"]
        });
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }

    public async Task<AppUser> CreateUserAsync(Guid userId = new(), string? email = null, string? password = null)
    {
        var user = new AppUser
        {
            Id = userId,
            UserName = "User",
            Email = email ?? "test@example.com"
        };

        using var scope = Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        var result = await userManager.CreateAsync(user, password ?? "Password@1");
        result.ShouldBe(IdentityResult.Success);

        return user;
    }

    /// <summary>
    /// Creates an instance of <see cref="HttpClient"/> with populated authorization header
    /// </summary>
    public HttpClient CreateAuthorizedClient(Guid? userId = null, string? userEmail = null)
    {
        userId ??= Guid.NewGuid();
        userEmail ??= "test@example.com";

        var user = new AppUser { Id = userId.Value, Email = userEmail };

        using var scope = Services.CreateScope();

        // Generate tokens
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
        var token = tokenService.GenerateTokens(user).AccessToken;

        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    private async Task MigrateDatabasesAsync()
    {
        var tagsDbContextOptions = new DbContextOptionsBuilder<TagsDbContext>()
            .UseSqlServer(_dbContainer.GetConnectionString())
            .ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning))
            .Options;
        var tagsContext = new TagsDbContext(tagsDbContextOptions);
        
        await tagsContext.Database.MigrateAsync();

        var usersDbContextOptions = new DbContextOptionsBuilder<UsersDbContext>()
            .UseSqlServer(_dbContainer.GetConnectionString())
            .ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning))
            .Options;
        var usersContext = new UsersDbContext(usersDbContextOptions);

        await usersContext.Database.MigrateAsync();
    }

    public async Task ResetDbAsync()
    {
        await _respawner.ResetAsync(_dbContainer.GetConnectionString());
    }
}