using Gms.Entity;
using Gms.Entity.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.IService
{
    public interface IFinanceInfoService:IBaseService<FinanceInfo>
    {
        IQueryable<FinanceInfo> LoadPagesEntities(FinanceSearch financeSearch, bool isDeleted);
    }
}
