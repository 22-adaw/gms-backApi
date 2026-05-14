using Gms.Common;
using Gms.Entity;
using Gms.Entity.DTO;
using Gms.Entity.Enum;
using Gms.IService;
using Gms.WebApi.Attributes;
using Gms.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace Gms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserInfoService userInfoService;
        private readonly IVipInfoService vipInfoService;
        private readonly IConfiguration configuration;
        private readonly IMemoryCache memoryCache;
        public LoginController(IUserInfoService userInfoService,IConfiguration configuration, IVipInfoService vipInfoService, IMemoryCache memoryCache)
        {
          this.userInfoService = userInfoService;
            this.configuration  = configuration;
            this.vipInfoService = vipInfoService;
            this.memoryCache = memoryCache;
        }
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [UnitOfWork]
        public async Task<IActionResult> UserLogin([FromBody] UserLoginDTO UserLoginDTO)
        {
            //1.校验密码
            if(memoryCache.Get("client"+ UserLoginDTO.UserPhone)==null)
            {
                return BadRequest(new ApiResult<string> { Code = 400, Message = "验证码失效，请重新获取", Data = null, Success = false });
            }
            if(memoryCache.Get("client" + UserLoginDTO.UserPhone)!.ToString()!= UserLoginDTO.Code)
            {
                return  BadRequest(new ApiResult<string> { Code = 400, Message = "验证码错误", Data = null, Success = false });
            }
            if (string.IsNullOrEmpty(UserLoginDTO.UserPhone) || string.IsNullOrEmpty(UserLoginDTO.UserPassword))
            {
                return BadRequest("手机号码或密码不能为空");
            }
            var user= await userInfoService.LoadEntities(u => u.UserPhone == UserLoginDTO.UserPhone).FirstOrDefaultAsync();
            if(user == null)
            {
                return NotFound(new ApiResult<string> { Code = 404, Message = "用户名或密码错误", Data = null, Success = false });
            }
            bool isPasswordRight= PasswordHelper.VerifyPassword(UserLoginDTO.UserPassword, user.UserPassword!);
            if (!isPasswordRight)
            {
                return BadRequest(new ApiResult<string> { Code = 400, Message = "密码错误", Data = null, Success = false });
            }
            if (user.IsDeleted == Convert.ToBoolean(DelFlagEnum.Deleted)&& user.IsClient==Convert.ToBoolean(ClientCheckEnum.Client))
            {
                return BadRequest(new ApiResult<string> { Code = 400, Message = "您的账号尚未激活，请前往邮箱激活或重新注册", Data = null, Success = false });
            }
            if(user.IsDeleted == Convert.ToBoolean(DelFlagEnum.Deleted))
            {
                return BadRequest(new ApiResult<string> { Code = 400, Message = "账号状态异常", Data = null, Success = false });
            }
            if(!user.UserPassword!.StartsWith("$2a$11$"))
            {
                //加密密码
                string hashedPassword = PasswordHelper.HashPassword(UserLoginDTO.UserPassword);
                user.UserPassword = hashedPassword;
                bool v = await userInfoService.UpdateEntity(user);
                if(v)
                {
                    //加密成功
                    //发送已加密邮件
                    await userInfoService.SendChangedPassword(user);

                }
                else
                {
                    //加密失败
                    //发送提示密码修改邮件
                    await userInfoService.SendChangePassword(user);
                }
            }
            // 2、创建JWT
            // 创建header,内部定义了JWT编码的算法
            var securityAlgorithm = SecurityAlgorithms.HmacSha256;
            // 创建payload,需要根据项目的需求来进行创建，例如可能会使用到用户的编号，用户名，邮箱等
            // 创建payload的内容，需要使用到Claim(每创建一个Claim对象，表示用户的一个信息)
            var claims = new[]
            {
                  // 第一项是，用户的ID，但是在JWT中，关于ID在JWT中有一个专用的名词叫做sub
                  new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub,user.Id.ToString())
            };
            // 创建签名，签名会使用到私钥，可以把私钥保存在配置文件中
            // 读取配置文件中的私钥，然后转成字节
            var secretByte = Encoding.UTF8.GetBytes(configuration["Authentication:SecretKey"]!);
            // 使用加密算法，对私钥进行加密
            var signingKey = new SymmetricSecurityKey(secretByte);
            // 构建数字签名
            var signingCredentials = new SigningCredentials(signingKey, securityAlgorithm);
            // 构建token 内容
            var token = new JwtSecurityToken(
                // 谁发布的token数据，一般是服务端的地址
                issuer: configuration["Authentication:Issuer"],
                 // 把token数据发布给谁，一般就是前端项目，这里也可以填写服务端的地址，或者不填写也可以
                 audience: configuration["Authentication:Audience"],
                claims,
                // 发布时间
                notBefore: DateTime.Now,
                // 有效期10天
                expires: DateTime.Now.AddDays(10),
                // 数字签名
                signingCredentials

                );
            // 将token生成字符串的形式进行输出
            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            //3、返回 200状态码和JWT内容
            return Ok(new ApiResult<string>() { Success=true,Message="登录成功", Data= tokenStr ,Code=200});

        }
        /// <summary>
        /// 找回密码
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        [HttpPost("FindPwd")]
        [UnitOfWork]
        public async Task<IActionResult> FindPwd([FromBody] FindPwdDTO findPwdDTO)
        {
            if (string.IsNullOrEmpty(findPwdDTO.Email))
            {
                return BadRequest(new ApiResult<string> { Code = 400, Message = "邮箱不能为空", Data = null, Success = false });
            }
            var user = await userInfoService.LoadEntities(u => u.UserEmail == findPwdDTO.Email && u.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            var vip= await vipInfoService.LoadEntities(v => v.VipEmail == findPwdDTO.Email && v.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if (user == null)
            {
                return BadRequest(new ApiResult<string>{ Code = 400, Message = "邮箱错误，未找到用户" ,Success=false,Data=null});
            }
            //生成临时密码，替换原密码
            string tempPassword = PasswordHelper.GeneratePassword();
            user.UserPassword = PasswordHelper.HashPassword(tempPassword);
            if(vip!=null)
            {
                vip.VipPassword = PasswordHelper.HashPassword(tempPassword);
                bool v2 = await vipInfoService.UpdateEntity(vip);
            }
            bool v1 = await userInfoService.UpdateEntity(user);
            UserFindDTO userFindDTO = new UserFindDTO()
            {
                UserPassword = tempPassword,
                RealName = user.RealName,
                UserEmail = user.UserEmail
            };
            bool v = await userInfoService.HandleFind(userFindDTO);
            // --------发送事件，向事件处理程序，发送数据，这里是LoginEvent对象
            //await mediator.Publish(new LoginEvent(user));
            if (v&& v1)
            {
                return Ok(new ApiResult<string> { Code = 200, Message = "密码已经发送至您的邮箱，请登录邮箱查看", Data = null, Success = true });
            }
            else
            {
                return BadRequest(new ApiResult<string> { Code = 400, Message = "无法验证安全性，请联系系统管理员解决", Data = null, Success = false });
            }
        }
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCode")]
        public async Task<IActionResult> GetCode([FromQuery] string userPhone)
        {
            var user = await userInfoService.LoadEntities(u => u.UserPhone == userPhone && u.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound(new ApiResult<string> { Code = 404, Message = "用户不存在或用户没有激活，请重新注册", Success = false, Data = null });
            }
            bool v = await userInfoService.HandleCode(user);
            if (v)
            {
                return Ok(new ApiResult<string> { Code = 200, Message = "验证码已发送至您绑定的邮箱，请注意查收", Data = null, Success = true });
            }
            else
            {
                return BadRequest(new ApiResult<string> { Code = 400, Message = "验证码发送失败，请重试", Data = null, Success = false });
            }
        }
    }
}
