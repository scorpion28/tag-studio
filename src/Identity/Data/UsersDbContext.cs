using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TagStudio.Identity.Domain;

namespace TagStudio.Identity.Data;

public class UsersDbContext(DbContextOptions<UsersDbContext> contextOptions)
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(contextOptions)
{
    public DbSet<UserRefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("identity");

        builder.Entity<AppUser>()
            .HasMany(u => u.RefreshTokens)
            .WithOne(u => u.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserRefreshToken>()
            .HasKey(rt => rt.Token);
    }
}