using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SkinScan_BL.Contracts
{
    public interface IGenericRepository<T> where T : class
    {
        Task AddedAsync(T entity);
        Task<IEnumerable<T>> GetByConditionAsync(Expression<Func<T, bool>> predicate);
       
        Task<T> GetByIdAsync(Expression<Func<T, bool>> match, string[] includes = null);

        Task<IEnumerable<T>> GetByNameAsync(Expression<Func<T, bool>> match, string[] includes = null);

        Task<IEnumerable<T>> GetAllAsync(string[] includes);

        Task UpdateAsync(Expression<Func<T, bool>> match, T entity);
        Task DeleteByIdAsync(Expression<Func<T, bool>> match);

        Task<IEnumerable<T>> OrderItems(Expression<Func<T, bool>> filter = null, Expression<Func<T, object>> orderBy = null, string[] includes = null);


    }
}