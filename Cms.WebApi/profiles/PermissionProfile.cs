using AutoMapper;
using Gms.Entity;
using Gms.Entity.DTO;

namespace Gms.WebApi.profiles
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            CreateMap<PermissionEditDTO, PermissionInfo>();//别忘了注入容器
        }
    }
}
