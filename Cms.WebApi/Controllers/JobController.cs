using Gms.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace Gms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly ISchedulerFactory schedulerFactory;

        public JobController(ISchedulerFactory schedulerFactory)
        {
            this.schedulerFactory = schedulerFactory;
        }
        /// <summary>
        /// 获取定时任务状态
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetStatus")]
        [Authorize]
        public async Task<IActionResult> GetStatus()
        {
            var scheduler = await schedulerFactory.GetScheduler();
            var jobKey = new JobKey("ExpiryReminderJob", "ReminderGroup");
            var triggers = await scheduler.GetTriggersOfJob(jobKey);
            
            if (!triggers.Any())
                return Ok(new ApiResult<object>() {Data= new { IsRunning = false, Message = "未找到触发器" },Code=200, Message= "获取定时任务状态成功",Success=true });

            var state = await scheduler.GetTriggerState(triggers.First().Key);
            return Ok(new ApiResult<object>() { Data = new{ IsRunning = state == TriggerState.Normal, State = state.ToString() } ,Code=200,Message="获取定时任务状态成功",Success=true});
        }
        /// <summary>
        /// 暂停定时任务
        /// </summary>
        /// <returns></returns>
        [HttpPost("Pause")]
        [Authorize]
        public async Task<IActionResult> Pause()
        {
            var scheduler = await schedulerFactory.GetScheduler();
            var jobKey = new JobKey("ExpiryReminderJob", "ReminderGroup");
            await scheduler.PauseJob(jobKey);
            return Ok(new ApiResult<string> { Data = null, Message = "任务已暂停" ,Success=true,Code=200});
        }
        /// <summary>
        /// 重启定时任务
        /// </summary>
        /// <returns></returns>
        [HttpPost("Resume")]
        [Authorize]
        public async Task<IActionResult> Resume()
        {
            var scheduler = await schedulerFactory.GetScheduler();
            var jobKey = new JobKey("ExpiryReminderJob", "ReminderGroup");
            await scheduler.ResumeJob(jobKey);
            return Ok(new ApiResult<string>(){Data=null, Message = "任务已恢复" ,Success=true,Code = 200 });
        }
        /// <summary>
        /// 立即执行一次
        /// </summary>
        /// <returns></returns>
        [HttpPost("TriggerNow")]
        [Authorize]
        public async Task<IActionResult> TriggerNow()
        {
            var scheduler = await schedulerFactory.GetScheduler();
            var jobKey = new JobKey("ExpiryReminderJob", "ReminderGroup");
            await scheduler.TriggerJob(jobKey);
            return Ok(new ApiResult<string>(){Code=200, Message = "任务已触发" ,Success=true,Data=null});
        }
    }
}
