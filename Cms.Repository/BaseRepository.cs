using Gms.Entity;
using Gms.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Repository
{
    public class BaseRepository<T> where T : class,new()
    {
        protected readonly MyDbContext myDbContext;
        public BaseRepository(MyDbContext myDbContext)
        {
            this.myDbContext = myDbContext;
        }
        public Task<bool> AddEntity(T entity)
        {
            myDbContext.Set<T>().Add(entity);
            return Task.FromResult(true);

        }

        public  Task<bool> DeleteEntity(T entity)
        {
           
            myDbContext.Set<T>().Remove(entity);
           // return await myDbContext.SaveChangesAsync()>0;
           return Task.FromResult(true);
        }

        public IQueryable<T> LoadEntities(Expression<Func<T, bool>> whereLambda)
        {
            return myDbContext.Set<T>().Where(whereLambda);
        }

        public IQueryable<T> LoadPagesEntities<S>(int pageIndex, int pageSize,out int totalCount, Expression<Func<T, bool>> whereLambda, Expression<Func<T, S>> orderbyLambda, bool isAsc)
        {
            var temp = myDbContext.Set<T>().Where(whereLambda);
            totalCount = temp.Count();
            if (isAsc)
            {
                // 升序 
                 temp = temp.OrderBy<T,S>(orderbyLambda).Skip<T>((pageIndex-1)*pageSize).Take<T>(pageSize);
            }
            else
            {
                // 降序
                temp = temp.OrderByDescending<T, S>(orderbyLambda).Skip<T>((pageIndex - 1) * pageSize).Take<T>(pageSize);
            }
            return temp;
        }

        public Task<bool> UpdateEntity(T entity)
        {
            myDbContext.Set<T>().Update(entity);
            return Task.FromResult(true);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await myDbContext.SaveChangesAsync();
        }
    }
}
