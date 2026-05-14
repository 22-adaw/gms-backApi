using Gms.Entity;
using Gms.Entity.DTO;
using Gms.Entity.Search;
using Gms.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.IService
{
    public interface IVipInfoService:IBaseService<VipInfo>
    {
        //已废弃
        //MyDbContext GetDbContext();

        /// <summary>
        /// 分页获取会员信息
        /// </summary>
        /// <param name="vipSearch"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        IQueryable<VipInfo> LoadPagesEntities(VipSearch vipSearch, bool isDeleted);
        Task<bool> HandleInform(VipInformDTO vipInformDTO);
        Task<bool> HandlePurchaseInform(PurchaseInformDTO purchaseInformDTO);
        Task<bool> HandleAddInform(AddInformDTO addInformDTO);
        Task<bool> AdminManifest(VipInfoDTO vipInfoDTO);
        Task<bool> ClientManifest(VipInfoDTO vipInfoDTO);
    }
}
