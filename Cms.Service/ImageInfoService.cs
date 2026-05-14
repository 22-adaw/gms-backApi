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
    public class ImageInfoService:BaseService<ImageInfo>,IImageInfoService
    {
        private readonly IImageInfoRepository imageInfoRepository;

        public ImageInfoService(IImageInfoRepository imageInfoRepository)
        {
            this.Repository= imageInfoRepository;
            this.imageInfoRepository = imageInfoRepository;
        }
    }
}
