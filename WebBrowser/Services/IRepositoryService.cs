using WebBrowser.Entities;

namespace WebBrowser.Services;

public interface IRepositoryService<T> where T : BaseEntity
{
    /// <summary>
    /// Returns all entities of type T
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();
    
    /// <summary>
    /// Adds an entity of type T
    /// </summary>
    /// <param name="entity">The entity that will be added</param>
    Task AddAsync(T entity);
    
    /// <summary>
    /// Removes an entity of type T using the specified id
    /// </summary>
    /// <param name="id">Id of the entity that will be removed</param>
    Task RemoveByIdAsync(string id);
    
    /// <summary>
    /// Removes all entity of type T
    /// </summary>
    /// <returns></returns>
    Task RemoveAllAsync();
    
    /// <summary>
    /// Updates an entity of type T
    /// </summary>
    /// <param name="entity">The entity that will be updated. Entity will be located using Id</param>
    Task UpdateAsync(T entity);
}