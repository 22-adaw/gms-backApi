using Gms.Entity;
using Gms.Entity.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.IService
{
    public interface IVipCardTypeService:IBaseService<VipCardType>
    {
        IQueryable<VipCardType> LoadPagesEntities(VipCardTypeSearch vipCardTypeSearch, bool isDeleted);
    }
}
