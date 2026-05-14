using Gms.Entity;
using Gms.Entity.Enum;
using Gms.Entity.Search;
using Gms.IRepository;
using Gms.IService;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Service
{
    public class FinanceInfoService:BaseService<FinanceInfo>,IFinanceInfoService
    {
        private readonly IFinanceInfoRepository financeInfoRepository;

        public FinanceInfoService(IFinanceInfoRepository financeInfoRepository)
        {
            base.Repository = financeInfoRepository;
            this.financeInfoRepository = financeInfoRepository;
        }
        /// <summary>
        /// 分页获取财务信息
        /// </summary>
        /// <param name="financeSearch"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        public IQueryable<FinanceInfo> LoadPagesEntities(FinanceSearch financeSearch, bool isDeleted)
        {
            var temp= financeInfoRepository.LoadEntities(f => f.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal));
            if(!string.IsNullOrEmpty(financeSearch.TypeName))
            {
                temp = temp.Where(f => f.TypeName.Contains(financeSearch.TypeName));
            }
            if(financeSearch.FinanceType==0||financeSearch.FinanceType==1)
            {
                temp = temp.Where(f => f.FinanceType == financeSearch.FinanceType);
            }
            if(financeSearch.RelatedCode!=0)
            {
                temp = temp.Where(f => f.RelatedCode==financeSearch.RelatedCode);
            }
            financeSearch.TotalCount = temp.Count();
            int skip = (financeSearch.PageIndex - 1) * financeSearch.PageSize;
            int take = financeSearch.PageSize;


            temp = financeSearch.Order
                ? temp.OrderBy(a => a.Id).ToList().Skip(skip).Take(take).AsQueryable()
                : temp.OrderByDescending(a => a.Id).ToList().Skip(skip).Take(take).AsQueryable();
            return temp;
        }
    }
}
