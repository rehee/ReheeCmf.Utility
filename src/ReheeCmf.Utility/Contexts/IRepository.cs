using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReheeCmf.Contexts
{
  public interface IRepository : ISaveChange
  {
    IQueryable<T> Query<T>(bool asNoTracking) where T : class;

    Task<T?> GetByIdAsync<T>(object id, CancellationToken cancellationToken = default(CancellationToken)) where T : class;

    Task AddAsync<T>(T entity, CancellationToken cancellationToken = default(CancellationToken)) where T : class;

    void Delete<T>(T entity) where T : class;

    Task DeleteAsync<T>(T entity) where T : class;

    void Delete(object entity);

    Task ExecuteTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default(CancellationToken));
  }
}
