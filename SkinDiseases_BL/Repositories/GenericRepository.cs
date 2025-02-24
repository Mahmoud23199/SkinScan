using Microsoft.EntityFrameworkCore;
using SkinScan_BL.Contracts;
using SkinScan_Core.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SkinScan_BL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public readonly SkinDbAppContext _context;

        public GenericRepository(SkinDbAppContext context)
        {
            this._context = context;
        }
        public async Task<IEnumerable<T>> GetByConditionAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
            {
                foreach (var include in includes)
                {

                    query = query.Include(include);
                }
            }
            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(Expression<Func<T, bool>> match, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
            {
                foreach (var include in includes)
                {

                    query = query.Include(include);
                }
            }
            return await query.FirstOrDefaultAsync(match);
        }

        public async Task<IEnumerable<T>> GetByNameAsync(Expression<Func<T, bool>> match, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.Where(match).ToListAsync();
        }

        public async Task UpdateAsync(Expression<Func<T, bool>> match, T entity)
        {
            //var existingEntity = await GetByIdAsync(match);

            //if (existingEntity != null)
            //{
            //    _context.Entry(existingEntity).CurrentValues.SetValues(entity);

            //    await _context.SaveChangesAsync();
            //}
            //else
            //{
            //    throw new ArgumentException("Entity not found", nameof(match));
            //}
            _context.Set<T>().Update(entity);

            await _context.SaveChangesAsync();
        }
        public async Task DeleteByIdAsync(Expression<Func<T, bool>> match)
        {
            var item = await GetByIdAsync(match);

            if (item != null)
            {
                _context.Set<T>().Remove(item);
                await _context.SaveChangesAsync();

            }
            else
                throw new KeyNotFoundException("Item not found");
        }

        public async Task AddedAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> OrderItems(Expression<Func<T, bool>> filter = null, Expression<Func<T, object>> orderBy = null, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (orderBy != null)
            {
                    query = query.OrderByDescending(orderBy);
            }
            return await query.ToListAsync();

        }


       

    }
}
