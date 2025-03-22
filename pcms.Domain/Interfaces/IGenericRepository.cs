using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pcms.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<bool> AddAsync(T entity);
        void Update(T entity);
       // Task DeleteAsync(string id);
        Task<int> SaveChangesAsync();
    }
}
