namespace TagStudio.Shared.Repository;

public interface IRepository<T>
{
    IUnitOfWork UnitOfWork { get; }

    Task<T> AddAsync(T book, CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}