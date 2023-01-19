using System.Reactive.Linq;
using Akavache;
using WebBrowser.Entities;

namespace WebBrowser.Services.Implementation;

public class RepositoryService<T> : IRepositoryService<T> where T : BaseEntity
{
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await BlobCache.LocalMachine.GetAllObjects<T>();
    }

    public async Task AddAsync(T entity)
    {
        await BlobCache.LocalMachine.InsertObject(entity.Id, entity);
    }

    public async Task RemoveAsync(T entity)
    {
        await BlobCache.LocalMachine.InvalidateObject<T>(entity.Id);
    }

    public async Task RemoveByIdAsync(string id)
    {
        await BlobCache.LocalMachine.InvalidateObject<T>(id);
    }

    public async Task RemoveAllAsync()
    {
        await BlobCache.LocalMachine.InvalidateAllObjects<T>();
    }
    
    public async Task UpdateAsync(T entity)
    {
        await BlobCache.LocalMachine.InsertObject(entity.Id, entity);
    }
}