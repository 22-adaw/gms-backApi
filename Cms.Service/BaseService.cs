using Gms.Entity;
using Gms.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Service
{
    public class BaseService<T> where T : class,new()
    {
        protected IBaseRepository<T> Repository { get; set; }  
        public async Task<bool> AddEntity(T entity)
        {
           return await  Repository.AddEntity(entity); 
        }

        public async Task<bool> DeleteEntity(T entity)
        {
            return await Repository.DeleteEntity(entity);
        }

        public IQueryable<T> LoadEntities(Expression<Func<T, bool>> whereLambda)
        {
            return  Repository.LoadEntities(whereLambda);
        }

        public IQueryable<T> LoadPagesEntities<S>(int pageIndex, int pageSize, out int totalCount, Expression<Func<T, bool>> whereLambda, Expression<Func<T, S>> orderbyLambda, bool isAsc)
        {
           return Repository.LoadPagesEntities(pageIndex,pageSize,out totalCount,whereLambda,orderbyLambda,isAsc);
        }

        public async Task<bool> UpdateEntity(T entity)
        {
           return await Repository.UpdateEntity(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Repository.SaveChangesAsync();
        }
    }
}
