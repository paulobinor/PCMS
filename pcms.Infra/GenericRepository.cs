using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

        public async Task<T> GetByIdAsync(string id)
        {
            try
            {
               var item =  await _dbSet.FindAsync(id);
                if (item == null) 
                {
                   // _logger.LogInformation($"Record not found for Id: {id}");
                    throw new Exception($"Record not found for Id: {id}");
                }
                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }
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

        }
        public async Task Update(T entity)
        {
            try
            {
                _dbSet.Update(entity);
            }
            catch (Exception)
            {

                throw;
            }
        }
       
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
