using DAL.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task Delete(T entity);
        Task<T> Get(int id);
        IQueryable<T> GetAll();
        Task<IList<T>> GetAllListAsync();
        void HardDelete(T entity);
        void HardDeleteRange(List<T> entities);
        T Insert(T entity);
        int InsertAndGetId(T entity);
        Task<int> InsertAndGetIdAsync(T entity);
        Task<T> InsertAsync(T entity);
        Task Update(T entity);
    }
}