using Backend.Infrastructures.Data;
using Backend.Infrastructures.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Backend.Infrastructures.Repositories.Implementation
{
    public abstract class BaseRepositoy<T> : IBaseRepository<T> where T : class
    {
        protected readonly CampusFestDbContext context;
        protected DbSet<T> Set => context.Set<T>();
        private bool disposedValue;

        public BaseRepositoy(CampusFestDbContext context)
        {
            this.context = context;
        }

        public async Task<T> Create(T entity)
        {
            await context.Set<T>().AddAsync(entity);

            await context.SaveChangesAsync();

            return entity;
        }

        public async Task CreateRange(List<T> entities)
        {
            await Set.AddRangeAsync(entities);

            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await Set.ToListAsync();
        }

        public async Task<T?> GetById<Tid>(Tid Id)
        {
            var target = await Set.FindAsync(Id);

            return target;
        }

        public async Task<IEnumerable<T>> GetPaginated(int page, int page_size)
        {
            return await Set.AsNoTracking().Skip((page - 1) * page_size).Take(page_size).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetPaginated(int page, int page_size, Expression<Func<T, bool>> filter, Expression<Func<T, object>> orderBy, bool IsAscending = false)
        {
            // Filter
            var result = Set.Where(filter);

            // Sort
            if (orderBy != null)
            {
                if (IsAscending)
                {
                    result = result.OrderBy(orderBy);
                }
                else
                {
                    result = result.OrderByDescending(orderBy);
                }
            }

            // Pagination
            return await result.Skip((page - 1) * page_size).Take(page_size).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetPaginated(int page, int page_size, Expression<Func<T, bool>> filter, string orderBy, bool IsAscending = false)
        {
            // Filter
            var result = Set.Where(filter);

            // Sort
            if (orderBy != null)
            {
                if (IsAscending)
                {
                    result = result.OrderBy(GetOrderByExpression(orderBy));
                }
                else
                {
                    result = result.OrderByDescending(GetOrderByExpression(orderBy));
                }
            }
           
            // Pagination
            return await result.Skip((page - 1) * page_size).Take(page_size).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetPaginated(int page, int page_size, Expression<Func<T, object>> includeProperty, Expression<Func<T, bool>> filter, Expression<Func<T, object>> orderBy, bool isDesending = false)
        {
            var result = Set.AsNoTracking().AsQueryable();

            // Filter
            if (filter != null)
            {
                result = result.Where(filter);
            }

            // Sort
            if (orderBy != null)
            {
                result = isDesending ? result.OrderByDescending(orderBy) : result.OrderBy(orderBy);
            }

            // Pagination
            result = result.Skip((page - 1) * page_size).Take(page_size);

            // Including items
            if (includeProperty != null)
            {
                result = result.Include(includeProperty);
            }

            return await result.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetPaginated(int page, int page_size, Expression<Func<T, object>> includeProperty, Expression<Func<T, bool>> filter, string orderBy, bool isDesending = false)
        {

            var result = Set.AsNoTracking().AsQueryable();

            // Filter
            if (filter != null)
            {
                result = result.Where(filter);
            }

            // Sort
            if (orderBy != null)
            {
                var order = GetOrderByExpression(orderBy);
                result = isDesending ? result.OrderByDescending(order) : result.OrderBy(order);
            }

            // Pagination

            result = result.Skip((page - 1) * page_size).Take(page_size);

            // Including items
            if (includeProperty != null)
            {
                result = result.Include(includeProperty);
            }

            return await result.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetPaginated(int page, int page_size, Expression<Func<T, object>> includeProperty, Expression<Func<T, bool>> filter, Expression<Func<T, object>> orderBy, bool tracking = false, bool isDesending = false)
        {
            var result = Set.AsQueryable();

            // Tracking
            if (tracking)
            {
                result = result.AsNoTracking();
            }

            // Filter
            if (filter != null)
            {
                result = result.Where(filter);
            }

            // Sort
            if (orderBy != null)
            {

                result = isDesending ? result.OrderByDescending(orderBy) : result.OrderBy(orderBy);
            }

            // Pagination

            result = result.Skip((page - 1) * page_size).Take(page_size);

            // Including items
            if (includeProperty != null)
            {
                result = result.Include(includeProperty);
            }

            return await result.ToListAsync();
        }

        private Expression<Func<T, object>> GetOrderByExpression(string key)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            var property = Expression.Property(parameter, key);

            var conversion = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(conversion, parameter);
        }

        public Task<bool> IsExist<Tvalue>(string key, Tvalue value)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            var property = Expression.Property(parameter, key);

            var constant = Expression.Constant(value);

            var equality = Expression.Equal(property, constant);

            var lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);

            return context.Set<T>().AnyAsync(lambda);
        }

        public Task<bool> IsExistForUpdate<Tid>(Tid id, string key, string value)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            var property = Expression.Property(parameter, key);

            var constant = Expression.Constant(value);

            var equality = Expression.Equal(property, constant);

            var idProperty = Expression.Property(parameter, "Id");

            var idEquality = Expression.NotEqual(idProperty, Expression.Constant(id));

            var combinedExpression = Expression.AndAlso(equality, idEquality);

            var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);

            return context.Set<T>().AnyAsync(lambda);
        }

        public async Task<bool> IsExist<Tid>(Tid id)
        {
            var result = await context.Set<T>().FindAsync(id);

            return result != null;
        }

        public async Task Remove(T entity)
        {
            context.Set<T>().Remove(entity);

            await context.SaveChangesAsync();
        }

        public async Task Update(T entity)
        {
            context.Set<T>().Update(entity);

            await context.SaveChangesAsync();
        }

        public async Task SaveChangeAsync()
        {
            await context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    context.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
