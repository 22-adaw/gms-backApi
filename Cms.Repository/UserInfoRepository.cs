using Gms.Entity;
using Gms.EntityFrameworkCore;
using Gms.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Repository
{
    public class UserInfoRepository :BaseRepository<UserInfo>, IUserInfoRepository
    {
        // 实现自己独有的方法。
        public UserInfoRepository(MyDbContext myDbContext):base(myDbContext) { }

    }
}
