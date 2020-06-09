using System.Collections.Generic;
using System.Threading.Tasks;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace ordercloud.integrations.library
{
    public interface ICosmosQuery<T> where T : ICosmosObject
    {
        Task<ListPage<T>> List(IListArgs args);
        Task<T> Get(string id);
        Task<T> Save(T entity);
        Task<List<T>> SaveMany(List<T> entities);
        Task Delete(string id);
    }
}
