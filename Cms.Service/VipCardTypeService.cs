using Gms.Entity;
using Gms.Entity.Enum;
using Gms.Entity.Search;
using Gms.IRepository;
using Gms.IService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Service
{
    public class VipCardTypeService:BaseService<VipCardType>,IVipCardTypeService
    {
        private readonly IVipCardTypeRepository vipCardTypeRepository;

        public VipCardTypeService(IVipCardTypeRepository vipCardTypeRepository)
        {
            base.Repository = vipCardTypeRepository;
            this.vipCardTypeRepository = vipCardTypeRepository;
        }
        /// <summary>
        /// 分页获取会员卡类别信息
        /// </summary>
        /// <param name="vipCardTypeSearch"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        public IQueryable<VipCardType> LoadPagesEntities(VipCardTypeSearch vipCardTypeSearch, bool isDeleted)
        {
            var temp= vipCardTypeRepository.LoadEntities(vct => vct.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal));
            if(!string.IsNullOrEmpty(vipCardTypeSearch.VipCardTypeName))
            {
                temp = temp.Where(t => t.VipCardTypeName.Contains(vipCardTypeSearch.VipCardTypeName));
            }
            if(vipCardTypeSearch.VipCardTypeCode==0|| vipCardTypeSearch.VipCardTypeCode==1)
            {
                temp = temp.Where(t => t.VipCardTypeCode == vipCardTypeSearch.VipCardTypeCode);
            }
            vipCardTypeSearch.TotalCount = temp.Count();
            int skip = (vipCardTypeSearch.PageIndex - 1) * vipCardTypeSearch.PageSize;
            int take = vipCardTypeSearch.PageSize;


            temp = vipCardTypeSearch.Order
                ? temp.OrderBy(a => a.Id).ToList().Skip(skip).Take(take).AsQueryable()
                : temp.OrderByDescending(a => a.Id).ToList().Skip(skip).Take(take).AsQueryable();
            return temp;
        }
    }
}
