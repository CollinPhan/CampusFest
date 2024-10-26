using Backend.Cores.Entities;
using System.Linq.Expressions;

namespace Backend.Infrastructures.Repositories.Interface
{
    public interface IBaseRepository<T> : IDisposable where T : class
    {
        Task<T?> GetById<Tid>(Tid Id);

        Task<IEnumerable<T>> GetAll();

        Task<IEnumerable<T>> GetPaginated(int page, int page_size);

        Task<IEnumerable<T>> GetPaginated(int page, int page_size, Expression<Func<T, bool>> filter, Expression<Func<T, object>> orderBy, bool isDesending = false);
        Task<IEnumerable<T>> GetPaginated(int page, int page_size, Expression<Func<T, bool>> filter, string orderBy, bool IsAscending = false);

        Task<IEnumerable<T>> GetPaginated(int page, int page_size, Expression<Func<T, object>> includeProperty, Expression<Func<T, bool>> filter, Expression<Func<T, object>> orderBy, bool isDesending = false);
        Task<IEnumerable<T>> GetPaginated(int page, int page_size, Expression<Func<T, object>> includeProperty, Expression<Func<T, bool>> filter, string orderBy, bool isDesending = false);

        Task<T> Create(T entity);

        Task CreateRange(List<T> entities);

        Task Update(T entity);

        Task Remove(T entity);
        Task<bool> IsExist<Tid>(Tid id);

        Task<bool> IsExist<Tvalue>(string property, Tvalue value);

        Task<bool> IsExistForUpdate<Tid>(Tid id, string key, string value);

        Task SaveChangeAsync();

    }
}
