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
    public interface IPermissionInfoService:IBaseService<PermissionInfo>
    {
        /// <summary>
        /// 添加权限
        /// </summary>
        /// <param name="permissionDto"></param>
        /// <returns></returns>
        Task<bool> AddPermission(PermissionDTO permissionDto);
        IQueryable<PermissionInfo> LoadSearchPageEntities(PermissionSearch permissionSearch,bool isDeleted);
        Task<bool> UpdateConfig(string key, string value);
        Task<bool> HandleSendDeletePwd(SendDeletePwdDTO sendDeletePwdDTO);
        Task<bool> HandleTriggerApproval(StartApproveDTO startApproveDTO);
        Task HandleSendEmail(PermissionDeleteApproval permissionDeleteApproval);
        //Task<bool> HandleSendEmailToRequester();
        Task<ApprovalProcessResult> ProcessApproval(string token, bool approved);
    }
}
