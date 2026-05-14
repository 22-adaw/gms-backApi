using AutoMapper;
using Gms.Entity;
using Gms.Entity.DTO;
using Gms.Entity.Enum;
using Gms.Entity.Search;
using Gms.IService;
using Gms.WebApi.Attributes;
using Gms.WebApi.Models;
using Gms.WebApi.SearchParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Gms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonService lessonService;
        private readonly IUserInfoService userInfoService;
        private readonly IVipInfoService vipInfoService;
        private readonly IVipCardService vipCardService;
        private readonly IMapper mapper;
        public LessonsController(ILessonService lessonService, IUserInfoService userInfoService,IVipInfoService vipInfoService, IVipCardService vipCardService, IMapper mapper)
        {
            this.lessonService = lessonService;
            this.userInfoService = userInfoService;
            this.vipInfoService = vipInfoService;
            this.vipCardService = vipCardService;
            this.mapper = mapper;
        }
        /// <summary>
        /// 分页获取课程信息
        /// </summary>
        /// <param name="lessonParams"></param>
        /// <returns></returns>
        [HttpGet("GetLessonsPages")]
        [Authorize]
        public IActionResult GetLessonsPages([FromQuery] LessonParams lessonParams)
        {
            var lessonSearch = new LessonSearch()
            {
                LessonName = lessonParams.LessonName,
                Status = lessonParams.Status,
                Order = lessonParams.Order,
                PageIndex = lessonParams.PageIndex,
                PageSize = lessonParams.PageSize,
                TotalCount = 0
            };
            var lessonsList = lessonService.LoadPagesEntities(lessonSearch, false).Select(l => new { LessonName = l.LessonName, LessonDesc = l.LessonDesc, Price = l.Price, Status = l.Status, CoachName = l.Couch == null ? "无教练" : l.Couch.RealName, Count = l.VipInfos.Count, MaxParticipants = l.MaxParticipants,Id=l.Id }).ToList();
            if (lessonsList.Count > 0)
            {
                return Ok(new ApiResult<object>() { Success = true, Message = "课程信息分页获取成功", Data = new { Rows = lessonsList, Total = lessonSearch.TotalCount }, Code = 200 });
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到课程信息", Data = null, Code = 404 });
            }
        }
        /// <summary>
        /// 获取教练
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCoachList")]
        [Authorize]
        public IActionResult GetCoachList()
        {
            //获取教练
            var coachList = userInfoService.LoadEntities(u => u.RoleInfos.Any(r => r.RoleName == "教练") && u.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Select(c => new { Id = c.Id, CoachName = c.RealName }).ToList();
            if (coachList.Count > 0)
            {
                return Ok(new ApiResult<object>() { Success = true, Message = "教练信息获取成功", Data = coachList, Code = 200 });
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到教练信息", Data = null, Code = 404 });
            }
        }
        /// <summary>
        /// 新建课程
        /// </summary>
        /// <param name="lessonDTO"></param>
        /// <returns></returns>
        [HttpPost("CreateLesson")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CreateLesson([FromBody] LessonDTO lessonDTO)
        {
            var coach=await userInfoService.LoadEntities(u => u.RealName == lessonDTO.CoachName && u.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            var lesson = new Lesson()
            {
                LessonName = lessonDTO.LessonName,
                LessonDesc = lessonDTO.LessonDesc,
                Couch = coach,
                MaxParticipants = lessonDTO.MaxParticipations,
                StartTime = lessonDTO.StartTime,
                EndTime = lessonDTO.EndTime,
                Price = lessonDTO.Price,
                IsDeleted = Convert.ToBoolean(DelFlagEnum.Nomal),
                CreateDate = DateTime.Now,
                Status = 0//未开始
            };
            bool v = await lessonService.AddEntity(lesson);
            if(v)
            {
                return Ok(new ApiResult<string>() { Success = true, Message = "新增课程成功", Data = null, Code = 200 });
            }
            else
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "新增课程失败", Data = null, Code = 400 });
            }
        }
        /// <summary>
        /// 根据id获取单个课程信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetLessonById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetLessonById([FromRoute]int id)
        {
             var lesson= await lessonService.LoadEntities(l => l.Id == id && l.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Select(l => new {LessonName=l.LessonName, LessonDesc=l.LessonDesc, MaxParticipants=l.MaxParticipants, StartTime=l.StartTime, EndTime=l.EndTime, Price=l.Price, Status=l.Status, CreateDate=l.CreateDate, EditDate=l.EditDate,Coach=new { CoachName= l.Couch.RealName,Id=l.CoachId} ,Id= l.Id, EnrolledCount=l.EnrolledCount }).FirstOrDefaultAsync();
            if(lesson!=null)
            {
                return Ok(new ApiResult<object>() { Success = true, Message = "获取课程成功", Data = lesson, Code = 200 });
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "获取课程失败", Data = null, Code = 404 });
            }
        }
        /// <summary>
        /// 根据id选课
        /// </summary>
        /// <param name="lessonChoseDTO"></param>
        /// <returns></returns>
        [HttpPost("ChoseLessonById")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> ChoseLessonById([FromBody] LessonChoseDTO lessonChoseDTO)
        {
            var lesson= await lessonService.LoadEntities(l => l.Id == lessonChoseDTO.CurrentId && l.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            var vip=await vipInfoService.LoadEntities(v => v.VipPhone == lessonChoseDTO.VipPhone && v.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Include(v=>v.VipCard).ThenInclude(v=>v.VipCardType).FirstOrDefaultAsync();
            if(lesson.EnrolledCount>=lesson.MaxParticipants)
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "参与人数已达到最大限制", Data = null, Code = 501 });
            }
            else if (vip==null||vip.VipCard == null|| vip.VipCard.EndDate<DateTime.Now)//判断是否有卡或会员卡是否有效
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "无会员信息、会员卡或卡到期不允许选课", Data = null, Code = 501 });
            }
            else if(vip.VipCard.FreezeStatus == 1 || vip.Status == "2")//判断会员和会员卡是否有效
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "选课失败，请确认您的会员状态或会员卡状态是否正常", Data = null, Code = 501 });

            }
            else if (vip.VipCard.LeftMoney < lesson!.Price)//判断余额是否充足
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "选课失败，余额不足，请充值", Data = null, Code = 501 });
            }
            else
            {
                lesson.EnrolledCount++;
                lesson.VipInfos.Add(vip);
                vip.VipCard.LeftMoney = vip.VipCard.LeftMoney - lesson.Price* vip.VipCard.VipCardType.DiscountRate;
                bool v1 = await vipCardService.UpdateEntity(vip.VipCard);
                vip.Lessons.Add(lesson);
                bool v = await vipInfoService.UpdateEntity(vip);
                var v2= await lessonService.UpdateEntity(lesson);
                if(v&&v1&&v2)
                {
                    return Ok(new ApiResult<string>() { Success = true, Message = "选课成功", Data = null, Code = 200 });
                }
                else
                {
                    return BadRequest(new ApiResult<string>() { Success = false, Message = "选课失败", Data = null, Code = 501 });
                }
            }
            
        }
        /// <summary>
        /// 获取该vip选择的课程信息
        /// </summary>
        /// <param name="VipPhone"></param>
        /// <returns></returns>
        [HttpGet("GetLessonChosedForVip/{VipPhone}")]
        [Authorize]
        public async Task<IActionResult> GetLessonChosedForVip([FromRoute]string VipPhone)
        {
            var user= await userInfoService.LoadEntities(u => u.UserPhone == VipPhone && u.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Include(u=>u.RoleInfos).FirstOrDefaultAsync();
            if (user.IsClient || user.RoleInfos.Select(r => r.Id).Contains(1))
            {
                var vip = await vipInfoService.LoadEntities(v => v.VipPhone == VipPhone && v.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Include(v => v.Lessons)!.ThenInclude(l => l.Couch).FirstOrDefaultAsync();

                if (vip == null)
                {
                    return BadRequest(new ApiResult<string>() { Success = false, Message = "您还不是会员", Data = null, Code = 404 });
                }
                if (vip.Lessons.Count < 0)
                {
                    return NotFound(new ApiResult<string>() { Success = false, Message = "您还没有选择任何课程", Data = null, Code = 404 });
                }
                else
                {
                    var lessonList = vip.Lessons.Where(l => l.IsDeleted == false).Select(l => new { Id = l.Id, CreateDate = l.CreateDate, LessonDesc = l.LessonDesc, EditDate = l.EditDate, MaxParticipants = l.MaxParticipants, CoachName = l.Couch.RealName, EndTime = l.EndTime, StartTime = l.StartTime, Status = l.Status, LessonName = l.LessonName, Price = l.Price , EnrolledCount=l.EnrolledCount}).ToList();
                    return Ok(new ApiResult<object>() { Success = true, Message = "获取该vip选择的课程成功", Data = lessonList, Code = 200 });

                }
            }
            else if (user.RoleInfos.Select(r => r.Id).Contains(4))
            {
                //返回当前教练发布的课程
                var lessonList= await lessonService.LoadEntities(l => l.CoachId == user.Id && l.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Select(l=>new { LessonName=l.LessonName, LessonDesc=l.LessonDesc, CoachName=l.Couch.RealName, MaxParticipants=l.MaxParticipants, EndTime = l.EndTime, StartTime = l.StartTime, Status = l.Status, Price = l.Price, EnrolledCount = l.EnrolledCount, Id = l.Id, CreateDate = l.CreateDate, EditDate = l.EditDate}).ToListAsync();

                return Ok(new ApiResult<object>() { Success = true, Message = "您是教练", Data = lessonList, Code = 200 });
            }
            else
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "无权查看", Data = null, Code = 400 });
            }
        }
        /// <summary>
        /// 取消选择课程
        /// </summary>
        /// <param name="lessonChoseDTO"></param>
        /// <returns></returns>
        [HttpPost("CancelLessonById")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CancelLessonById([FromBody] LessonChoseDTO lessonChoseDTO)
        {
            var vip=await vipInfoService.LoadEntities(v => v.VipPhone == lessonChoseDTO.VipPhone && v.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Include(v=>v.Lessons).FirstOrDefaultAsync();
            var vipCard= await vipCardService.LoadEntities(vc => vc.Id == vip.VipCardId && !vc.IsDeleted).Include(vc=>vc.VipCardType).FirstOrDefaultAsync();
            var lesson=await lessonService.LoadEntities(l => l.Id == lessonChoseDTO.CurrentId).FirstOrDefaultAsync();
            if(vip==null)
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到该会员", Data = null, Code = 404 });
            }
            if(lesson==null)
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到该课程", Data = null, Code = 404 });
            }
            lesson.EnrolledCount--;
            lesson.VipInfos.Remove(vip);
            vip.Lessons.Remove(lesson);
            //返还折后金额
            vipCard.LeftMoney = vipCard.LeftMoney + lesson.Price* vipCard.VipCardType.DiscountRate;
            bool v = await vipCardService.UpdateEntity(vipCard);
            bool v1 = await vipInfoService.UpdateEntity(vip);
            var v2= await lessonService.UpdateEntity(lesson);
            if(v&&v1&& v2&& lessonChoseDTO.IsAdmitHandle == 1)
            {
                return Ok(new ApiResult<string>() { Success = true, Message = "提出成功", Data = null, Code = 200 });
            } else if(v && v1 && v2 && lessonChoseDTO.IsAdmitHandle == 0)
            {
                return Ok(new ApiResult<string>() { Success = true, Message = "取消选择成功", Data = null, Code = 200 });
            }
            else
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "操作失败", Data = null, Code = 501 });
            }
        }
        /// <summary>
        /// 根据id编辑课程
        /// </summary>
        /// <param name="lessonEditDTO"></param>
        /// <returns></returns>
        [HttpPost("EditLessonById")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> EditLessonById([FromBody] LessonEditDTO lessonEditDTO)
        {
            //查出教练
            var coach= await userInfoService.LoadEntities(u => u.Id == lessonEditDTO.CoachId && u.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(coach==null)
            {
                return NotFound(new ApiResult<string> { Code=404,Message="未找到该教练",Data=null,Success=false});
            }
            //查出课程
            var lesson= await lessonService.LoadEntities(l => l.Id == lessonEditDTO.Id && l.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(lesson==null)
            {
                return NotFound(new ApiResult<string> { Code = 404, Message = "未找到该该课程", Data = null, Success = false });
            }
            //自动映射
             mapper.Map(lessonEditDTO,lesson);
            lesson.Couch = coach;
            lesson.EditDate = DateTime.Now;
            //保存到数据库
            bool v = await lessonService.UpdateEntity(lesson);
            if(v)
            {
                return Ok(new ApiResult<string> { Code = 200, Message = "编辑课程成功", Data = null, Success = true });
            }
            else
            {
                return BadRequest(new ApiResult<string> { Code = 400, Message = "编辑课程失败", Data = null, Success = false });
            }
        }
        /// <summary>
        /// 根据id软删除课程
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteLessonById/{id}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> DeleteLessonById([FromRoute]int id)
        {
            //查询出要删除的课程
            var lesson= await lessonService.LoadEntities(l => l.Id == id && l.IsDeleted==Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            
            if(lesson==null)
            {
                return NotFound(new ApiResult<string> { Code = 404, Message = "未找到要删除的课程", Data = null, Success = false });
            }
            if (lesson.Status == 1)
            {
                return BadRequest(new ApiResult<string> { Code = 400, Message = "该课程正在进行中，无法删除", Data = null, Success = false });
            }
            lesson.IsDeleted = Convert.ToBoolean(DelFlagEnum.Deleted);
            bool v = await lessonService.UpdateEntity(lesson);
            if(v)
            {
                return Ok(new ApiResult<string> { Code = 200, Message = "删除课程成功", Data = null, Success = true });
            }
            else
            {
                return BadRequest(new ApiResult<string> { Code = 400, Message = "删除课程失败", Data = null, Success = false });
            }
        }
        /// <summary>
        /// 根据id更改课程状态
        /// </summary>
        /// <param name="lessonStatusChangeDTO"></param>
        /// <returns></returns>
        [HttpPatch("ChangeStatusById")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> ChangeStatusById([FromBody]LessonStatusChangeDTO lessonStatusChangeDTO)
        {
            //查出当前课程
            var lesson= await lessonService.LoadEntities(l => l.Id == lessonStatusChangeDTO.Id && l.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(lesson==null)
            {
                return NotFound(new ApiResult<string> { Code = 404, Message = "未找到要改变状态的课程", Data = null, Success = false });
            }
            lesson.Status = lessonStatusChangeDTO.Status;
            lesson.EditDate = DateTime.Now;
            //保存到数据库
            bool v = await lessonService.UpdateEntity(lesson);
            if (v)
            {
                return Ok(new ApiResult<string> { Code = 200, Message = "状态更改成功", Data = null, Success = true });
            }
            else
            {
                return BadRequest(new ApiResult<string> { Code = 400, Message = "状态更改失败", Data = null, Success = false });
            }
        }
        /// <summary>
        /// 根据Id获取当前课程选择学员
        /// </summary>
        /// <param name="lessonId"></param>
        /// <returns></returns>
        [HttpGet("getNowParticipatedVips")]
        [Authorize]
        public async Task<IActionResult> getNowParticipatedVips([FromQuery]int lessonId)
        {
            var vipList= await lessonService.LoadEntities(l => l.Id == lessonId && l.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Select(l=>l.VipInfos).ToListAsync();
            if(vipList.Count<=0)
            {
                return BadRequest(new ApiResult<string> { Code = 400, Message = "暂无学员", Data = null, Success = false });
            }
            return Ok(new ApiResult<object> { Code = 200, Message = "获取学员成功", Data = vipList, Success = true });
        }
    }
}
