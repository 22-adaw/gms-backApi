using Gms.Common;
using Gms.Entity;
using Gms.Entity.DTO;
using Gms.Entity.Enum;
using Gms.Entity.Search;
using Gms.EntityFrameworkCore;
using Gms.IService;
using Gms.Service;
using Gms.WebApi.Attributes;
using Gms.WebApi.Models;
using Gms.WebApi.SearchParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Gms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VipInfosController : ControllerBase
    {
        private readonly IVipInfoService vipInfoService;
        private readonly IVipCardService vipCardService;
        private readonly IFinanceInfoService financeInfoService;
        private readonly IUserInfoService userInfoService;
        private readonly IConfiguration configuration;
        private readonly IRoleInfoService roleInfoService;
        private readonly IMemoryCache memoryCache;
        public VipInfosController(IVipInfoService vipInfoService, IVipCardService vipCardService, IFinanceInfoService financeInfoService, IConfiguration configuration, IUserInfoService userInfoService, IRoleInfoService roleInfoService, IMemoryCache memoryCache)
        {
            this.vipInfoService = vipInfoService;
            this.vipCardService = vipCardService;
            this.financeInfoService = financeInfoService;
            this.configuration = configuration;
            this.userInfoService = userInfoService;
            this.roleInfoService = roleInfoService;
            this.memoryCache = memoryCache;
        }
        /// <summary>
        /// 分页获取会员信息
        /// </summary>
        /// <param name="vipParams"></param>
        /// <returns></returns>
        [HttpGet("GetVipInfoPages")]
        [Authorize]
        public async Task<IActionResult> GetVipInfoPages([FromQuery]VipParams vipParams)
        {
            VipSearch vipSearch = new VipSearch()
            {
                VipPhone = vipParams.VipPhone,
                VipName = vipParams.VipName,
                Order = vipParams.Order,
                PageIndex = vipParams.PageIndex,
                PageSize = vipParams.PageSize,
                TotalCount = 0,
                VipEmail=vipParams.VipEmail
            };
            var vipInfoList =await vipInfoService.LoadPagesEntities(vipSearch, false).Select(v => new { Id = v.Id, Gender = v.Gender, VipPhone = v.VipPhone, VipEmail = v.VipEmail, Status = v.Status, VipName = v.VipName ,VipCardId=v.VipCardId, CardNum = v.VipCard != null ? (v.VipCard.CardNum ?? 0) : 0, FreezeStatus=v.VipCard==null? 0: v.VipCard.FreezeStatus }).ToListAsync();
            if(vipInfoList.Count>0)
            {
                return Ok(new ApiResult<object>() { Success = true, Message = "会员信息分页获取成功", Data = new { Rows = vipInfoList, Total = vipSearch.TotalCount }, Code = 200 });
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到会员信息", Data = null, Code = 404 });
            }
        }
        /// <summary>
        /// 新增会员/会员注册
        /// </summary>
        /// <param name="vipInfoDTO"></param>
        /// <returns></returns>
        [HttpPost("CreateVipInfo")]
        [UnitOfWork]
        public async Task<IActionResult> CreateVipInfo([FromBody]VipInfoDTO vipInfoDTO)
        {
            //校验该账号是否已存在
            var isExist=  userInfoService.LoadEntities(u =>( u.UserPhone == vipInfoDTO.VipPhone||u.UserEmail==vipInfoDTO.VipEmail)&&u.IsDeleted==Convert.ToBoolean(DelFlagEnum.Nomal)).Any();
            if(isExist)
            {
                return Ok(new ApiResult<string> { Code = 200, Message = "注册失败，该账号已存在", Data = null, Success = false });
            }
            bool v= await vipInfoService.AdminManifest(vipInfoDTO);
            if(v)
            {
                return Ok(new ApiResult<string>() { Success = true, Message = $"新增会员成功,已发送邮件", Data = null, Code = 200 });
            }
            else
            {
                bool v1= await vipInfoService.ClientManifest(vipInfoDTO);
                if(v1)
                {
                    return Ok(new ApiResult<string>() { Success = true, Message = $"为了保护您的账户安全，我们向{vipInfoDTO.VipEmail}发送了一封电子邮件，请点击邮件中的链接完成激活。", Data = null, Code = 200 });
                }
                else
                {
                    return BadRequest(new ApiResult<string>() { Success = false, Message = "注册失败，请重试", Data = null, Code = 400 });
                }
            }


            #region 已废弃代码
            //随机生成密码
            //var password= PasswordHelper.GeneratePassword();
            //var vipInfo = new VipInfo()
            //{
            //    VipEmail = vipInfoDTO.VipEmail,
            //    VipName = vipInfoDTO.VipName,
            //    VipPhone = vipInfoDTO.VipPhone,
            //    Gender = vipInfoDTO.Gender,
            //    IsDeleted = Convert.ToBoolean(DelFlagEnum.Nomal),
            //    Status = "0",
            //    CreateDate = DateTime.Now,
            //    VipPassword = string.IsNullOrEmpty(vipInfoDTO.VipPassword) ? PasswordHelper.HashPassword(password) : PasswordHelper.HashPassword(vipInfoDTO.VipPassword)
            //};

            //bool v = await vipInfoService.AddEntity(vipInfo);
            ////必须先保存到会员表，否则无法获取该会员的自增id 
            //await vipInfoService.SaveChangesAsync();

            //var role= await roleInfoService.LoadEntities(r => r.Id == 8 && r.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).ToListAsync();
            //var user = new UserInfo()
            //{
            //    Id = vipInfo.Id,
            //    UserName = vipInfoDTO.VipName,
            //    UserPassword = vipInfo.VipPassword,
            //    UserEmail = vipInfoDTO.VipEmail,
            //    UserPhone = vipInfoDTO.VipPhone,
            //    PhotoUrl = "images\\aa.jpg",
            //    Gender = Convert.ToInt32(vipInfoDTO.Gender),
            //    CreateDate = DateTime.Now,
            //    EditDate = DateTime.Now,
            //    RealName = vipInfoDTO.VipName,
            //    RoleInfos = role,
            //    IsDeleted = Convert.ToBoolean(DelFlagEnum.Nomal),
            //    DepartmentId = 32,
            //    IsClient = Convert.ToBoolean(ClientCheckEnum.Client)
            //};
            //string str = "";
            //if(vipInfoDTO.IsAdminManifest)
            //{
            //    //发送邮件给客户，告知账号已开通
            //    VipInformDTO vipInformDTO = new VipInformDTO()
            //    {
            //        UserName= vipInfoDTO.VipName,
            //        Password = password,
            //        Phone = vipInfoDTO.VipPhone,
            //        Email = vipInfoDTO.VipEmail,
            //    };
            //    await vipInfoService.HandleInform(vipInformDTO);
            //    str = $"新增会员成功,已发送邮件到{user.UserEmail}";
            //}
            //else
            //{
            //    user.IsDeleted = Convert.ToBoolean(DelFlagEnum.Deleted);
            //    await userInfoService.HandleActive(user);
            //    str =$"为了保护您的账户安全，我们向{user.UserEmail}发送了一封电子邮件，以验证您的身份，请点击邮件中的链接继续。";
            //}
            //bool v1 = await userInfoService.AddEntity(user);

            //if (v&&v1)
            //{
            //    return Ok(new ApiResult<string>() { Success = true, Message = str, Data = null, Code = 200 });
            //}
            //else
            //{
            //    return BadRequest(new ApiResult<string>() { Success = false, Message = "新增会员失败", Data = null, Code = 400 });
            //}
            #endregion
        }
        /// <summary>
        /// 根据id获取会员信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetVipInfoById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetVipInfoById([FromRoute]int id)
        {
            var vipInfo = await vipInfoService.LoadEntities(v => v.Id == id && v.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Include(v => v.VipCard).ThenInclude(vc => vc.VipCardType).Include(v => v.VipCard).Select(v => new { Id=v.Id, VipEmail=v.VipEmail, Status=v.Status, VipName= v.VipName, VipPhone=v.VipPhone, VipCardTypeName = v.VipCard != null ? v.VipCard.VipCardType.VipCardTypeName : null, CardNum = v.VipCard != null ? v.VipCard.CardNum : null, Gender=v.Gender, VipCardId=v.VipCardId, FreezeStatus = v.VipCard != null ? v.VipCard.FreezeStatus : (int?)null, RemainTimes = v.VipCard != null ? v.VipCard.RemainTimes : (int?)null, StartDate = v.VipCard != null ? v.VipCard.StartDate : (DateTime?)null, EndDate = v.VipCard != null ? v.VipCard.EndDate : (DateTime?)null, LeftMoney = v.VipCard != null ? v.VipCard.LeftMoney : (float?)null }).FirstOrDefaultAsync();
            var userPhoto= await userInfoService.LoadEntities(u => u.Id == vipInfo.Id && u.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Select(u=>u.PhotoUrl).FirstOrDefaultAsync();
            if (vipInfo != null)
            {
                return Ok(new ApiResult<object>() { Success = true, Message = "获取会员信息成功", Data =new { VipInfo = vipInfo,PhotoUrl=userPhoto }, Code = 200 });
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到会员信息", Data = null, Code = 404 });
            }
        }
        /// <summary>
        /// 根据id软删除会员
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteVipInfoById/{id}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> DeleteVipInfoById([FromRoute]int id)
        {
            var vipInfo= await vipInfoService.LoadEntities(v => v.Id == id && v.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(vipInfo!=null)
            {
                vipInfo.IsDeleted = true;
                vipInfo.EditDate = DateTime.Now;
                bool v= await vipInfoService.UpdateEntity(vipInfo);
                if(v)
                {
                    return Ok(new ApiResult<string>() { Success = true, Message = "删除会员成功", Data = null, Code = 200 });
                }
                else
                {
                    return BadRequest(new ApiResult<string>() { Success = false, Message = "删除会员失败", Data = null, Code = 400 });
                }
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到要删除的会员", Data = null, Code = 404 });
            }
        }
        /// <summary>
        /// 根据id跟新会员信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="vipInfoDTO"></param>
        /// <returns></returns>
        [HttpPatch("EditVipInfoById/{id}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> EditVipInfoById([FromRoute]int id, [FromBody]VipInfoDTO vipInfoDTO)
        {
            var vipInfo= await vipInfoService.LoadEntities(v => v.Id == id && v.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            var user= await userInfoService.LoadEntities(u => u.Id == id && u.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(vipInfo!=null&& user!=null)
            {
                vipInfo.VipEmail = vipInfoDTO.VipEmail;
                vipInfo.VipPhone = vipInfoDTO.VipPhone;
                vipInfo.VipName = vipInfoDTO.VipName;
                vipInfo.Gender = vipInfoDTO.Gender;
                vipInfo.EditDate = DateTime.Now;
                vipInfo.Status = vipInfoDTO.Status;

                user.UserEmail= vipInfoDTO.VipEmail;
                user.UserPhone= vipInfoDTO.VipPhone;
                user.UserName = vipInfoDTO.VipName;
                user.Gender= Convert.ToInt32(vipInfoDTO.Gender);
                user.EditDate = DateTime.Now;
                bool v1 = await userInfoService.UpdateEntity(user);
                bool v = await vipInfoService.UpdateEntity(vipInfo);
                if(v&&v1)
                {
                    return Ok(new ApiResult<string>() { Success = true, Message = "会员信息更新成功", Data = null, Code = 200 });
                }
                else
                {
                    return BadRequest(new ApiResult<string>() { Success = false, Message = "会员信息更新失败", Data = null, Code = 400 });
                }
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到要更新的会员信息", Data = null, Code = 404 });
            }
            
        }
        #region 已废弃代码
        /// <summary>
        /// 会员办卡(已废弃)
        /// </summary>
        //[HttpPost("PurchaseVipCard/{id}")]
        //[Authorize]
        //public IActionResult async Task<IActionResult> PurchaseVipCard([FromRoute] int id, [FromBody] VipCardType vipCardType)
        //{
        //    IDbContextTransaction dbContextTransaction = null;
        //    try
        //    {
        //        var dbContext = vipInfoService.GetDbContext();
        //        dbContextTransaction = await dbContext.Database.BeginTransactionAsync();

        //        var vipInfo = await vipInfoService.LoadEntities(v => v.Id == id && v.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();

        //        if (vipInfo != null)
        //        {
        //            var vipCard = new VipCard()
        //            {
        //                StartDate = DateTime.Now,
        //                EndDate = DateTime.Now.AddDays((double)vipCardType.UseDays),
        //                CreateTime = DateTime.Now,
        //                RemainTimes = vipCardType.UseTimes,
        //                FreezeStatus = 0,
        //                VipCardTypeId = vipCardType.Id,
        //                CardNum = new Random().Next(10000000, 99999999)
        //            };

        //            vipInfo.VipCard = vipCard;

        //            var financeInfo = new FinanceInfo()
        //            {
        //                FinanceType = 0,
        //                TypeName = "办卡收入",
        //                Amount = vipCardType.Price,
        //                FinanceCode = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999),
        //                RelatedCode = vipCard.CardNum,
        //                RelatedType = "vip",
        //                IsDeleted = Convert.ToBoolean(DelFlagEnum.Nomal),
        //                Remark = vipCardType.Remark,
        //                CreateDate = DateTime.Now
        //            };
        //            await vipCardService.AddEntity(vipCard);
        //            await financeInfoService.AddEntity(financeInfo);
        //            await vipInfoService.UpdateEntity(vipInfo);
        //            //统一提交
        //            await dbContext.SaveChangesAsync();

        //            await dbContextTransaction.CommitAsync();
        //            return Ok(new ApiResult<string>() { Success = true, Message = "会员卡购买成功", Data = null, Code = 200 });
        //        }
        //        else
        //        {
        //            return NotFound(new ApiResult<string>() { Success = false, Message = "未找到要购买会员卡的会员", Data = null, Code = 404 });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (dbContextTransaction != null)
        //        {
        //              //统一回滚
        //            await dbContextTransaction.RollbackAsync();
        //        }
        //        return BadRequest(new ApiResult<string>() { Success = false, Message = "购买会员卡失败：" + ex.Message, Data = null, Code = 410 });
        //    }
        //    finally
        //    {
        //        if (dbContextTransaction != null)
        //        {
        //            await dbContextTransaction.DisposeAsync();
        //        }
        //    }
        //}
        #endregion
        /// <summary>
        /// 办卡
        /// </summary>
        /// <param name="id"></param>
        /// <param name="vipCardType"></param>
        /// <returns></returns>
        [HttpPost("PurchaseVipCard/{id}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> PurchaseVipCard([FromRoute] int id, [FromBody] VipCardType vipCardType)
        {
           var vip= await vipInfoService.LoadEntities(v => v.Id == id && v.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Include(v=>v.VipCard).ThenInclude(v=>v.VipCardType).FirstOrDefaultAsync();
            VipCard newCard = null;
            if (vip.VipCard!=null)
            {
                //说明该用户之前已经办过卡了，查出他的卡
                var vipCard= await vipCardService.LoadEntities(vc => vc.Id == vip.VipCardId && vc.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
                

                TimeSpan? ts = vipCard.EndDate - DateTime.Now;
                //创建一个新卡
                 newCard = new VipCard()
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays((double)vipCardType.UseDays) + ts,
                    CreateTime = DateTime.Now,
                    RemainTimes = vipCardType.UseTimes+ vipCard.RemainTimes,
                    FreezeStatus = 0,
                    VipCardTypeId = vipCardType.Id,
                    LeftMoney = 0 + vipCard.LeftMoney,
                    CardNum = new Random().Next(10000000, 99999999)
                };
                //禁用旧卡
                vipCard.IsDeleted = Convert.ToBoolean(DelFlagEnum.Deleted);
                vipCard.EndDate = DateTime.Now;
                vipCard.LeftMoney = 0;
                vipCard.RemainTimes = 0;
                
                await vipCardService.UpdateEntity(vipCard);
                
            }
            else
            {
                //没办过卡。创建一个新卡
                 newCard = new VipCard()
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays((double)vipCardType.UseDays),
                    CreateTime = DateTime.Now,
                    RemainTimes = vipCardType.UseTimes,
                    FreezeStatus = 0,
                    VipCardTypeId = vipCardType.Id,
                    LeftMoney = 0 ,
                    CardNum = new Random().Next(10000000, 99999999)
                };

            }
            //生成财务数据
            var financeInfo = new FinanceInfo()
            {
                FinanceType = 0,
                TypeName = "办卡收入",
                Amount = vipCardType.Price,
                FinanceCode = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999),
                RelatedCode = newCard.CardNum,
                RelatedType = "vip",
                IsDeleted = Convert.ToBoolean(DelFlagEnum.Nomal),
                Remark = vipCardType.Remark,
                CreateDate = DateTime.Now
            };
            vip.VipCard = newCard;
            //事务提交，保存到数据库
            bool v2 = await financeInfoService.AddEntity(financeInfo);
            bool v1 = await vipCardService.AddEntity(newCard);
            bool v = await vipInfoService.UpdateEntity(vip);
            if(v1&&v2&&v)
            {
                //发送邮件
                PurchaseInformDTO purchaseInformDTO = new PurchaseInformDTO()
                {
                    CardNum = newCard.CardNum,
                    CardTypeName = vipCardType.VipCardTypeName,
                    DisCountRate = vipCardType.DiscountRate,
                    Email = vip.VipEmail,
                    EndDate = newCard.EndDate,
                    LeftMoney = newCard.LeftMoney,
                    RemainTime = newCard.RemainTimes,
                    Remark = vipCardType.Remark,
                    VipName=vip.VipName
                };
                await vipInfoService.HandlePurchaseInform(purchaseInformDTO);
                return Ok(new ApiResult<string>() { Success = true, Message = "会员卡购买成功", Data = null, Code = 200 });
            }
            else
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "购买会员卡失败：" , Data = null, Code = 410 });
            }
        }
        #region 已废弃代码
        /// <summary>
        /// Vip登录（已废弃）
        /// </summary>
        /// <param name="vipInfo"></param>
        /// <returns></returns>
        //[HttpPost("VipLogin")]
        //public async Task<IActionResult> VipLogin([FromBody]VipInfo vipInfo)
        //{
        //    if (string.IsNullOrEmpty(vipInfo.VipPhone) || string.IsNullOrEmpty(vipInfo.VipPassword))
        //    {
        //        return BadRequest("手机号码或密码不能为空");
        //    }

        //    var user = await vipInfoService.LoadEntities(u => u.VipPhone == vipInfo.VipPhone && u.VipPassword == vipInfo.VipPassword).FirstOrDefaultAsync();
        //    if (user == null)
        //    {
        //        return BadRequest("手机号码或密码错误");
        //    }

        //    // 2、创建JWT
        //    // 创建header,内部定义了JWT编码的算法
        //    var securityAlgorithm = SecurityAlgorithms.HmacSha256;
        //    // 创建payload,需要根据项目的需求来进行创建，例如可能会使用到用户的编号，用户名，邮箱等
        //    // 创建payload的内容，需要使用到Claim(每创建一个Claim对象，表示用户的一个信息)
        //    var claims = new[]
        //    {
        //          // 第一项是，用户的ID，但是在JWT中，关于ID在JWT中有一个专用的名词叫做sub
        //          new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub,user.Id.ToString())
        //      };
        //    // 创建签名，签名会使用到私钥，可以把私钥保存在配置文件中
        //    // 读取配置文件中的私钥，然后转成字节
        //    var secretByte = Encoding.UTF8.GetBytes(configuration["Authentication:SecretKey"]!);
        //    // 使用加密算法，对私钥进行加密
        //    var signingKey = new SymmetricSecurityKey(secretByte);
        //    // 构建数字签名
        //    var signingCredentials = new SigningCredentials(signingKey, securityAlgorithm);
        //    // 构建token 内容
        //    var token = new JwtSecurityToken(
        //        // 谁发布的token数据，一般是服务端的地址
        //        issuer: configuration["Authentication:Issuer"],
        //         // 把token数据发布给谁，一般就是前端项目，这里也可以填写服务端的地址，或者不填写也可以
        //         audience: configuration["Authentication:Audience"],
        //        claims,
        //        // 发布时间
        //        notBefore: DateTime.Now,
        //        // 有效期10天
        //        expires: DateTime.Now.AddDays(10),
        //        // 数字签名
        //        signingCredentials

        //        );
        //    // 将token生成字符串的形式进行输出
        //    var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
        //    //3、返回 200状态码和JWT内容
        //    return Ok(new ApiResult<string>() { Success = true, Message = "登录成功", Data = tokenStr ,Code=200});

        //}
        #endregion
        /// <summary>
        /// 会员充值
        /// </summary>
        /// <param name="id"></param>
        /// <param name="addMoneyDTO"></param>
        /// <returns></returns>
        [HttpPost("addMoney/{id}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> addMoney([FromRoute]int id, [FromBody]AddMoneyDTO addMoneyDTO) 
        {
            var vip= await vipInfoService.LoadEntities(v => v.Id == id && v.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Include(v=>v.VipCard).FirstOrDefaultAsync();
            if(!PasswordHelper.VerifyPassword(addMoneyDTO.VipPassword, vip.VipPassword))
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "充值失败,密码错误", Data = null, Code = 400 });
            }
            else if(vip.VipCard==null)
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "充值失败,无有效会员卡", Data = null, Code = 400 });
            }
            else if(vip.VipCard.EndDate < DateTime.Now)
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "充值失败,会员卡到期", Data = null, Code = 400 });
            }
            else if (vip.VipCard.FreezeStatus == 1)
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "充值失败,会员卡已被冻结", Data = null, Code = 400 });
            }
            else if (addMoneyDTO.Amount <= 0)
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "充值失败,金额不能小于0", Data = null, Code = 400 });
            }
            else
            {
                //密码正确
                vip.VipCard.LeftMoney += addMoneyDTO.Amount;
                await vipInfoService.UpdateEntity(vip);
                int v = await vipInfoService.SaveChangesAsync();
                //插入数据到财务信息表
                var financeInfo = new FinanceInfo()
                {
                    FinanceType = 0,
                    TypeName = "充值收入",
                    Amount = (double)addMoneyDTO.Amount,
                    FinanceCode = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999),
                    RelatedCode = vip.VipCard.CardNum,
                    RelatedType = "vipAddMoney",
                    Remark = $"充值{addMoneyDTO.Amount}元",
                    CreateDate = DateTime.Now,
                    IsDeleted = Convert.ToBoolean(DelFlagEnum.Nomal)
                };
                bool v1 = await financeInfoService.UpdateEntity(financeInfo);
                if (v > 0 && v1)
                {
                    //发送充值成功邮件通知
                    AddInformDTO addInformDTO = new AddInformDTO()
                    {
                        Amount = addMoneyDTO.Amount,
                        CardNum = vip.VipCard.CardNum,
                        Email = vip.VipEmail,
                        LeftMoney = vip.VipCard.LeftMoney,
                        VipName= vip.VipName
                    };
                    //异步发送邮件，不等待
                    await vipInfoService.HandleAddInform(addInformDTO);
                    return Ok(new ApiResult<string>() { Success = true, Message = "充值成功", Data = null, Code = 200 });
                }
                else
                {
                    return BadRequest(new ApiResult<string>() { Success = false, Message = "充值失败", Data = null, Code = 400 });
                }
            }
        }
        /// <summary>
        /// 冻结/解冻会员卡
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("FreezeOrUnFreezeCard/{id}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> FreezeOrUnFreezeCard([FromRoute]int id)
        {
            //获取用户会员卡
            var vipCard= await vipInfoService.LoadEntities(v => v.Id == id && v.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Include(v=>v.VipCard).Select(v => v.VipCard).FirstOrDefaultAsync();
            if(vipCard==null)
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到会员卡", Data = null, Code = 404 });
            }
            //0表示未冻结，1表示冻结
            vipCard.FreezeStatus = vipCard.FreezeStatus == 0 ? 1 : 0;
            string statueStr = vipCard.FreezeStatus == 0 ? "解冻成功":"冻结成功";
            //保存到数据库
            bool v = await vipCardService.UpdateEntity(vipCard);
            
            if(v)
            {
                return Ok(new ApiResult<string>() { Success = true, Message = statueStr, Data = null, Code = 200 });
            }
            else
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = statueStr, Data = null, Code = 400 });
            }
        }
        #region 已废弃代码
        /// <summary>
        /// 激活账号(页面太丑，已废弃)
        /// </summary>
        /// <param name="email"></param>
        /// <param name="activeCode"></param>
        /// <returns></returns>
        //[HttpGet("activeAccount")]
        //[UnitOfWork]
        //public async Task<IActionResult> ActiveAccount([FromQuery] string email, [FromQuery] string activeCode)
        //{
        //    if (memoryCache.Get("user" + email) == null)
        //    {
        //        return BadRequest(new ApiResult<string>{ Code = 400, Message = "激活码失效，请重新注册",Success=false,Data=null });
        //    }
        //    if (memoryCache.Get("user" + email)!.ToString() != activeCode)
        //    {
        //        return BadRequest(new ApiResult<string> { Code = 400, Message = "激活码错误，请重试", Success = false, Data = null });
        //    }
        //    var user = await userInfoService.LoadEntities(u => u.UserEmail == email && u.IsDeleted == Convert.ToBoolean(DelFlagEnum.Deleted)).FirstOrDefaultAsync();
        //    if (user != null)
        //    {
        //        user.IsDeleted = Convert.ToBoolean(DelFlagEnum.Nomal);
        //        return Ok(new ApiResult<string> { Code = 200, Message = "恭喜您激活成功，本页面无法自动关闭，请手动关闭本页面,注册流程将在您关闭本页面时正式关闭" ,Success=true,Data=null});
        //    }
        //    else
        //    {
        //        return BadRequest(new ApiResult<string> { Code = 400, Message = "很遗憾您激活失败，请手动关闭本页面重试", Success = false, Data = null });
        //    }
        //}
        #endregion
        /// <summary>
        /// 激活账号(美化版)
        /// </summary>
        /// <param name="email"></param>
        /// <param name="activeCode"></param>
        /// <returns></returns>
        [HttpGet("activeAccount")]
        [UnitOfWork]
        public async Task<IActionResult> ActiveAccount([FromQuery] string email, [FromQuery] string activeCode, [FromQuery]string phone)
        {
            // 准备返回的HTML模板
            string GetResultPage(string title, string message, string status, string detail = "")
            {
                return $@"
<!DOCTYPE html>
<html lang='zh-CN'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0, user-scalable=no'>
    <title>{title} - 健身房管理系统</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Microsoft YaHei', 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
            padding: 20px;
        }}
        .card {{
            background: white;
            border-radius: 16px;
            box-shadow: 0 20px 60px rgba(0,0,0,0.3);
            max-width: 500px;
            width: 100%;
            padding: 40px;
            text-align: center;
            animation: fadeInUp 0.6s ease-out;
        }}
        @keyframes fadeInUp {{
            from {{
                opacity: 0;
                transform: translateY(30px);
            }}
            to {{
                opacity: 1;
                transform: translateY(0);
            }}
        }}
        .icon {{
            width: 80px;
            height: 80px;
            margin: 0 auto 20px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 48px;
        }}
        .icon.success {{
            background: #d4edda;
            color: #28a745;
        }}
        .icon.error {{
            background: #f8d7da;
            color: #dc3545;
        }}
        .icon.warning {{
            background: #fff3cd;
            color: #ffc107;
        }}
        h1 {{
            font-size: 24px;
            color: #333;
            margin-bottom: 12px;
        }}
        .message {{
            font-size: 16px;
            color: #666;
            line-height: 1.6;
            margin: 20px 0;
        }}
        .detail {{
            font-size: 14px;
            color: #999;
            background: #f8f9fa;
            padding: 12px;
            border-radius: 8px;
            margin: 20px 0;
            word-break: break-all;
        }}
        .btn {{
            display: inline-block;
            padding: 12px 30px;
            background: #0F4C81;
            color: white;
            text-decoration: none;
            border-radius: 25px;
            font-size: 16px;
            margin-top: 10px;
            transition: all 0.3s ease;
            border: none;
            cursor: pointer;
        }}
        .btn:hover {{
            background: #0a3a62;
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(15,76,129,0.3);
        }}
        .btn.secondary {{
            background: #6c757d;
        }}
        .btn.secondary:hover {{
            background: #5a6268;
        }}
        .countdown {{
            font-size: 14px;
            color: #999;
            margin-top: 20px;
        }}
        .footer {{
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px solid #eee;
            font-size: 12px;
            color: #bbb;
        }}
        @@media (max-width: 480px) {{
            .card {{
                padding: 30px 20px;
            }}
            h1 {{
                font-size: 20px;
            }}
        }}
    </style>
</head>
<body>
    <div class='card'>
        <div class='icon {status}'>{(status == "success" ? "✓" : (status == "error" ? "✗" : "⚠"))}</div>
        <h1>{title}</h1>
        <div class='message'>{message}</div>
        {(string.IsNullOrEmpty(detail) ? "" : $"<div class='detail'>{detail}</div>")}
        <button class='btn' onclick='window.close()'>关闭页面</button>
        <div class='countdown' id='countdown'></div>
        <div class='footer'>健身房管理系统 · 账号安全中心</div>
    </div>
    <script>
        let seconds =20;
        const countdownEl = document.getElementById('countdown');
        const closeBtn = document.querySelector('.btn');
        
        function updateCountdown() {{
            if (seconds > 0) {{
                countdownEl.innerHTML = seconds + ' 秒后自动关闭本页面...';
                seconds--;
                setTimeout(updateCountdown, 1000);
            }} else {{
                countdownEl.innerHTML = '页面即将关闭...';
                window.close();
            }}
        }}
        updateCountdown();
        
        closeBtn.onclick = function() {{
            window.close();
        }};
    </script>
</body>
</html>";
            }

            //验证激活码是否失效（从缓存取）
            string cacheKey = $"register:{phone}";
            if (!memoryCache.TryGetValue(cacheKey, out RegisterCacheData registerData))
            {
                return Content(
                    GetResultPage("激活失败", "激活链接已失效", "error", "激活码已过期（有效期为10分钟），请重新注册获取新的激活链接。"),
                    "text/html"
                );
            }

            // 2. 验证激活码是否正确
            if (registerData.ActiveCode != activeCode)
            {
                return Content(
                    GetResultPage("激活失败", "激活码错误", "error", "您的激活码不正确，请检查链接完整性或重新注册。"),
                    "text/html"
                );
            }

            //验证邮箱是否匹配
            if (registerData.VipEmail != email)
            {
                return Content(
                    GetResultPage("激活失败", "邮箱不匹配", "error", "激活链接与注册邮箱不一致，请重新注册。"),
                    "text/html"
                );
            }

            //检查是否已经被激活过了（防止重复激活）
            var existingUser = await userInfoService.LoadEntities(u => u.UserPhone == registerData.VipPhone && u.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                memoryCache.Remove(cacheKey);
                return Content(
                    GetResultPage("激活失败", "账号已激活", "warning", "该账号已被激活，请直接登录系统。"),
                    "text/html"
                );
            }

            // 5. 激活成功，创建用户到数据库
            string finalPassword = string.IsNullOrEmpty(registerData.VipPassword)
                ? PasswordHelper.GeneratePassword()
                : registerData.VipPassword;

            var vipInfo = new VipInfo()
            {
                VipEmail = registerData.VipEmail,
                VipName = registerData.VipName,
                VipPhone = registerData.VipPhone,
                Gender = registerData.Gender,
                IsDeleted = Convert.ToBoolean(DelFlagEnum.Nomal),
                Status = "0",
                CreateDate = DateTime.Now,
                VipPassword = PasswordHelper.HashPassword(finalPassword)
            };

            await vipInfoService.AddEntity(vipInfo);
            await vipInfoService.SaveChangesAsync();

            var role = await roleInfoService.LoadEntities(r => r.Id == 8 && r.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).ToListAsync();
            var user = new UserInfo()
            {
                Id = vipInfo.Id,
                UserName = registerData.VipName,
                UserPassword = vipInfo.VipPassword,
                UserEmail = registerData.VipEmail,
                UserPhone = registerData.VipPhone,
                PhotoUrl = "images\\aa.jpg",
                Gender = Convert.ToInt32(registerData.Gender),
                CreateDate = DateTime.Now,
                EditDate = DateTime.Now,
                RealName = registerData.RealName ?? registerData.VipName,
                RoleInfos = role,
                IsDeleted = Convert.ToBoolean(DelFlagEnum.Nomal),
                DepartmentId = 32,
                IsClient = Convert.ToBoolean(ClientCheckEnum.Client)
            };

            await userInfoService.AddEntity(user);

            //清除缓存
            memoryCache.Remove(cacheKey);

            //返回激活成功页面
            return Content(
                GetResultPage("激活成功", "恭喜您，账号已成功激活！", "success", "您现在可以登录健身房管理系统。本页面将自动关闭。"),
                "text/html"
            );
        }
        /// <summary>
        /// 根据手机号获取当前会员
        /// </summary>
        /// <param name="userPhone"></param>
        /// <returns></returns>
        [HttpGet("GetVipInfoByPhone")]
        [Authorize]
        public async Task<IActionResult> GetVipInfoByPhone([FromQuery] string userPhone)
        {
            var vipInfo = await vipInfoService.LoadEntities(v => v.VipPhone == userPhone && v.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Include(v => v.VipCard).ThenInclude(vc => vc.VipCardType).Include(v => v.VipCard).Select(v => new {v.Id,v.VipEmail, v.Status,v.VipName,v.VipPhone,VipCardTypeName = v.VipCard != null ? v.VipCard.VipCardType.VipCardTypeName : null,CardNum = v.VipCard != null ? v.VipCard.CardNum : null,v.Gender,v.VipCardId,FreezeStatus = v.VipCard != null ? v.VipCard.FreezeStatus : (int?)null,RemainTimes = v.VipCard != null ? v.VipCard.RemainTimes : (int?)null,StartDate = v.VipCard != null ? v.VipCard.StartDate : (DateTime?)null,EndDate = v.VipCard != null ? v.VipCard.EndDate : (DateTime?)null, LeftMoney = v.VipCard != null ? v.VipCard.LeftMoney : (float?)null}).FirstOrDefaultAsync();
                if (vipInfo != null)
            {
                return Ok(new ApiResult<object>() { Success = true, Message = "获取会员信息成功", Data =new { VipInfo=vipInfo ,PhotoUrl=(string)null}, Code = 200 });
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到会员信息", Data = null, Code = 404 });
            }
        }
    }
}
