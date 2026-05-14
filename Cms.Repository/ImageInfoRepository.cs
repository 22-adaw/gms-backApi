using Gms.Entity;
using Gms.EntityFrameworkCore;
using Gms.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Repository
{
    public class ImageInfoRepository : BaseRepository<ImageInfo>, IImageInfoRepository
    {
        public ImageInfoRepository(MyDbContext myDbContext) : base(myDbContext)
        {
        }
    }
}
