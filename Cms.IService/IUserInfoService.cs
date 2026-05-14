using Gms.Entity;
using Gms.Entity.DTO;
using Gms.Entity.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.IService
{
    public interface IUserInfoService:IBaseService<UserInfo>
    {
        /// <summary>
        /// 分页获取用户信息
        /// </summary>
        /// <param name="userSearch">分页条件</param>
        /// <param name="isDeleted">是否已被删除</param>
        /// <returns></returns>
        IQueryable<UserInfo> LoadPageEntities(UserSearch userSearch,bool isDeleted);
        /// <summary>
        /// 给用户设置角色
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="list">角色列表</param>
        /// <returns></returns>
        Task<bool> SetUserRoles(UserInfo userInfo, List<long> list);
        /// <summary>
        /// 获取用户的角色权限信息
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <returns></returns>
        Task<Dictionary<string, List<string>>> GetUserPermission(UserInfo userInfo);
        //Task<bool> HandleActive(UserInfo userInfo);
        Task<bool> HandleFind(UserFindDTO userFindDTO);
        Task<bool> HandleCreate(UserInDTO userInDTO);
        Task<bool> HandleCode(UserInfo userInfo);
        Task<bool> SendChangePassword(UserInfo userInfo);
        Task<bool> SendChangedPassword(UserInfo userInfo);
    }
}
