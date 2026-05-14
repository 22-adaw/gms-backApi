using AutoMapper;
using Cms.Entity;
using Cms.IService;
using Cms.Web.Attributes;
using Cms.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Cms.Web.Controllers
{
    public class UserController : Controller
    {

        private readonly IUserInfoService userInfoService;
        private readonly IMapper mapper;
        public UserController(IUserInfoService userInfoService, IMapper mapper) {
         this.userInfoService = userInfoService;
            this.mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetUsers()
        {
            var users = await userInfoService.LoadEntities(a => a.IsDeleted == false).ToListAsync();
           var userDtos = mapper.Map<List<UserInfoDto>>(users);
            return Json(userDtos);
        }

        [UnitOfWork]
        public async Task<IActionResult> CreateUser()
        {
            UserInfo userInfo = new UserInfo()
            {
                IsDeleted = false,
                CreateDate = DateTime.Now,
                EditDate = DateTime.Now,
                Gender = 0,
                PhotoUrl = "images/aa.jpg",
                UserEmail = "zhangsan@126.com",
                UserName = "zhangsan",
                UserPassword = "123456",
                UserPhone = "123456"
            };
          await  userInfoService.AddEntity(userInfo);
            return Content("ok");

        }
    }
}
