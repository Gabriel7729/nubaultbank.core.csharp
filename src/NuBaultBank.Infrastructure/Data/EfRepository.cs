using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using NuBaultBank.SharedKernel.Interfaces;

namespace NuBaultBank.Infrastructure.Data;

// inherit from Ardalis.Specification type
public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T> where T : class, IAggregateRoot
{
  private readonly DbContext _dbContext;
  private IDbContextTransaction? _currentTransaction;
  public EfRepository(AppDbContext dbContext) : base(dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
  {
    _dbContext.Set<T>().AddRange(entities);

    await SaveChangesAsync(cancellationToken);

    return entities;
  }

  public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
  {
    if (_currentTransaction is not null)
      return;

    _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
  }
  public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
  {
    if (_currentTransaction is null)
      return;

    await _currentTransaction.CommitAsync(cancellationToken);
  }
  public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
  {
    if (_currentTransaction is null)
      return;

    await _currentTransaction.RollbackAsync(cancellationToken);
    await _currentTransaction.DisposeAsync();
  }
  public async Task DisposeTransactionAsync()
  {
    if (_currentTransaction is null)
      return;

    await _currentTransaction.DisposeAsync();
  }
}
