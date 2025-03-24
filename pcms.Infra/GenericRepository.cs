using Microsoft.EntityFrameworkCore;
using pcms.Domain.Interfaces;
using System;

namespace pcms.Infra
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDBContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDBContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> GetByIdAsync(string id) => await _dbSet.FindAsync(id);
        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task AddAsync(T entity)
        {
            try
            {
               await _dbSet.AddAsync(entity);
            }
            catch (Exception)
            {
                throw;
            }
          //var res = await _context.SaveChangesAsync();
          //  if (res > 0)
          //  {
          //      return true;
          //  }
          //  else
          //  {
          //      return false;
          //  }
        }
        public void Update(T entity) => _dbSet.Update(entity);
       
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
