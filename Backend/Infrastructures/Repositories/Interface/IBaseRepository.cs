using Backend.Cores.Entities;
using System.Linq.Expressions;

namespace Backend.Infrastructures.Repositories.Interface
{
    public interface IBaseRepository<T> : IDisposable where T : class
    {
        /// <summary>
        /// Return a record by it's Id (primary key) if found, else return null.
        /// Note: As using Code First approach, each entity table will have exactly one primary key.
        /// </summary>
        /// <typeparam name="Tid">The type of the record primary key</typeparam>
        /// <param name="Id">The value of the record primary key</param>
        /// <returns><seealso cref="{T}"/> if found, else <seealso cref="null"/></returns>
        Task<T?> GetById<Tid>(Tid Id);

        /// <summary>
        /// Return the first found record (no sorting) which has it's property that match the requirement if found, else null.
        /// </summary>
        /// <typeparam name="Tvalue"></typeparam>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns><seealso cref="{T}"/> if found, else <seealso cref="null"/></returns>
        Task<T?> FindFirstMatch<Tvalue>(string property, Tvalue value);

        /// <summary>
        /// Return the first found record (no sorting) that's sastisfies the input condition if found, else return null.
        /// </summary>
        /// <param name="predicate">The condition for the search</param>
        /// <returns><seealso cref="{T}"/> if found, else <seealso cref="null"/></returns>
        Task<T?> FindFirstMatch(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Return all record of the entity set in the database.
        /// </summary>
        /// <returns><seealso cref="IEnumerable{T}"/></returns>
        Task<IEnumerable<T>> GetAll();

        /// <summary>
        /// Return all sorted (if any) record that match the filter criteria in a single list with fixed size page configuration.
        /// This is a method to use since it provide all neccessary parameters for fully customized pagination query.
        /// </summary>
        /// <param name="page">The entity result list current page.</param>
        /// <param name="page_size">The entity result list page size</param>
        /// <param name="includeProperty">The list of property's name which will be included in the entity.</param>
        /// <param name="filter">The filter criteria for searching</param>
        /// <param name="orderBy">The entity property used for sorting the result list.</param>
        /// <param name="tracking">Should the return list of entity be tracked</param>
        /// <param name="isDesending">Should the list be in descending order when sorted ?</param>
        /// <returns><seealso cref="IEnumerable{T}"/></returns>
        Task<IEnumerable<T>> GetPaginated(int page, int page_size, IEnumerable<string>? includeProperty = null, Expression<Func<T, bool>>? filter = null, Expression<Func<T, object>>? orderBy = null, bool tracking = false, bool isDesending = false);

        /// <summary>
        /// Add a new entity record and save all changes.
        /// </summary>
        /// <param name="entity">The entity to be added</param>
        /// <returns></returns>
        Task<T> Create(T entity);

        /// <summary>
        /// Add all entity records inside the list then save all changes.
        /// </summary>
        /// <param name="entities">The list of entities to be added</param>
        /// <returns></returns>
        Task<IEnumerable<T>> CreateRange(IEnumerable<T> entities);

        /// <summary>
        /// Update an entity record.
        /// <para>Note: Using an UNTRACKED entity will throw an <seealso cref="Exception"/>.</para>
        /// <para>Advised against using entity which found using pagination for this method. If must, use the pagination method with full parameter</para>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task Update(T entity);

        /// <summary>
        /// Delete an entity record
        /// </summary>
        /// <param name="entity">The entity to be removed.</param>
        /// <returns></returns>
        Task Remove(T entity);

        /// <summary>
        /// Check if there is a record with the primary key equals to the provided id.
        /// </summary>
        /// <typeparam name="Tid">The datatype of the primary key</typeparam>
        /// <param name="id">The primary key value</param>
        /// <returns>true if the record is found, else false</returns>
        Task<bool> IsExist<Tid>(Tid id);

        /// <summary>
        /// Check if there is a record has the property with the same value as input.
        /// </summary>
        /// <typeparam name="Tvalue">The property datatype</typeparam>
        /// <param name="property">The property name</param>
        /// <param name="value">The property value</param>
        /// <returns>true if the record is found, else false</returns>
        Task<bool> IsExist<Tvalue>(string property, Tvalue value);

        /// <summary>
        /// Check if there is any record has the property with the same value as input, excluding the record with provided id.
        /// </summary>
        /// <typeparam name="Tid"></typeparam>
        /// <param name="id"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> IsExistForUpdate<Tid>(Tid id, string key, string value);

        /// <summary>
        /// Return the count of all entities in the dataset.
        /// </summary>
        /// <returns></returns>
        Task<int> Count();

        /// <summary>
        /// Return the count of all entities in the data set that match the predicate.
        /// </summary>
        /// <param name="predicate">The search requirement which record to use</param>
        /// <returns></returns>
        Task<int> Count(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Save all changes made. 
        /// </summary>
        /// <returns></returns>
        Task SaveChangeAsync();

    }
}
