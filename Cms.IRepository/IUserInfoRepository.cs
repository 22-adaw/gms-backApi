using Gms.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Gms.IRepository
{
    public interface IUserInfoRepository:IBaseRepository<UserInfo>
    {
        // 定义声明自己独有的方法。
    }
}
