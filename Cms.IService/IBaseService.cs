using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Gms.IService
{
    public interface IBaseService<T> where T : class,new()
    {
        IQueryable<T> LoadEntities(Expression<Func<T, bool>> whereLambda);
        IQueryable<T> LoadPagesEntities<S>(int pageIndex, int pageSize, out int totalCount, Expression<Func<T, bool>> whereLambda, Expression<Func<T, S>> orderbyLambda, bool isAsc);

        Task<bool> AddEntity(T entity);

        Task<bool> DeleteEntity(T entity);

        Task<bool> UpdateEntity(T entity);

        Task<int> SaveChangesAsync();
    }
}
