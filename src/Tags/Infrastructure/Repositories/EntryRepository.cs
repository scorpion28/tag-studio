using TagStudio.Shared.Repository;
using TagStudio.Tags.Data;
using TagStudio.Tags.Domain;

namespace TagStudio.Tags.Infrastructure.Repositories;

public class EntryRepository(TagsDbContext context) : IEntryRepository
{
    public IUnitOfWork UnitOfWork { get; } = context;

    public async Task<Entry> AddAsync(Entry entry, CancellationToken cancellationToken = default)
    {
        var entityEntry = await context.Entries.AddAsync(entry, cancellationToken);
        return entityEntry.Entity;
    }

    public async Task<Entry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Entries.FindAsync([id], cancellationToken);
    }

    public async Task DeleteAsync(Entry entry, CancellationToken cancellationToken = default)
    {
        context.Entries.Remove(entry);
        
        await UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}