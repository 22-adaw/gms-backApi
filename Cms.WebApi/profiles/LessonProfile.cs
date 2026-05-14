using AutoMapper;
using Gms.Entity;
using Gms.Entity.DTO;

namespace Gms.WebApi.profiles
{
    public class LessonProfile : Profile
    {
        public LessonProfile()
        {
            //只覆盖LessonEditDTO存在的字段，其他不存在的保持原值
            CreateMap<LessonEditDTO, Lesson>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); 
        }
    }
}
