using Backend.Infrastructures.Data;
using Backend.Infrastructures.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Backend.Infrastructures.Repositories.Implementation
{
    public class BaseRepositoy<T> : IBaseRepository<T> where T : class
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

        public async Task<IEnumerable<T>> CreateRange(IEnumerable<T> entities)
        {
            await Set.AddRangeAsync(entities);

            await context.SaveChangesAsync();

            return entities;
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
        
        public async Task<IEnumerable<T>> GetPaginated(int page, int page_size, IEnumerable<string>? includeProperty, Expression<Func<T, bool>>? filter = null, Expression<Func<T, object>>? orderBy = null, bool tracking = false, bool isDesending = false)
        {
            // The result set
            IQueryable<T> result = Set.AsQueryable();

            // Filter
            if (filter != null)
            {
                result = result.Where(filter).AsQueryable();
            }

            // Include
            if (includeProperty != null)
            {
                foreach (string property in includeProperty)
                {
                    result = result.Include(property);
                }
            }
            
            // Sort
            if (orderBy != null)
            {
                if (isDesending)
                {
                    result = result.OrderByDescending(orderBy).AsQueryable();
                }
                else
                {
                    result = result.OrderBy(orderBy).AsQueryable();
                }
                
            }

            // Tracking (for updating and deleting purposes)
            if (!tracking)
            {
                result = result.AsNoTracking();
            }

            // Return pagination result
            return await result.ToListAsync();
        }

        private Expression<Func<T, object>> GetOrderByExpression(string key)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            var property = Expression.Property(parameter, key);

            var conversion = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(conversion, parameter);
        }

        public async Task<T?> FindFirstMatch<Tvalue>(string key, Tvalue value)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, key);
            var constant = Expression.Constant(value, typeof(Tvalue));
            BinaryExpression body = Expression.Equal(property, constant);
            Expression<Func<T, bool>> predicate = Expression.Lambda<Func<T, bool>>(body, parameter);

            var result = await Set.Where(predicate).FirstOrDefaultAsync();

            return result;
        }

        public async Task<T?> FindFirstMatch(Expression<Func<T, bool>> predicate)
        {
            return await Set.Where(predicate).FirstOrDefaultAsync();
        }

        public async Task<T?> FindFirstMatch(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> includes)
        {
            return await Set.Where(predicate).Include(includes).FirstOrDefaultAsync();
        }

        public async Task<bool> IsExist<Tid>(Tid id)
        {
            var result = await context.Set<T>().FindAsync(id);

            return result != null;
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

        public async Task<int> Count()
        {
            return await Set.CountAsync();
        }

        public async Task<int> Count(Expression<Func<T, bool>> predicate)
        {
            return await Set.Where(predicate).CountAsync(predicate);
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
