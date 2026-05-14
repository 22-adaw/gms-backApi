using Gms.Entity;
using Gms.IRepository;
using Gms.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Service
{
    public class VipCardService:BaseService<VipCard>,IVipCardService
    {
        private readonly IVipCardRepository vipCardRepository;

        public VipCardService(IVipCardRepository vipCardRepository)
        {
            base.Repository = vipCardRepository;
            this.vipCardRepository = vipCardRepository;
        }
    }
}
