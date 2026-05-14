using AutoMapper;
using Cms.Entity;
using Cms.Web.Models;

namespace Cms.Web.Profiles
{
    public class UserInfoProfile:Profile
    {
        public UserInfoProfile() {

            CreateMap<UserInfo, UserInfoDto>();
        }
    }
}
