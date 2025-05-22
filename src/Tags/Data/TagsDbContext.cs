using Microsoft.EntityFrameworkCore;
using TagStudio.Tags.Domain;

namespace TagStudio.Tags.Data;

public class TagsDbContext(DbContextOptions<TagsDbContext> options) : DbContext(options)
{
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Entry> Entries { get; set; }

    public DbSet<TagParent> TagParents { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Tag>()
            .HasMany(t => t.Children)
            .WithMany(t => t.Parents)
            .UsingEntity<TagParent>(
                r => r.HasOne(tp => tp.Child).WithMany(),
                l => l.HasOne(tp => tp.Parent).WithMany());
    }
}