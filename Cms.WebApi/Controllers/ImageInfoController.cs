using Gms.Common;
using Gms.Entity;
using Gms.Entity.Enum;
using Gms.IService;
using Gms.WebApi.Attributes;
using Gms.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Gms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageInfoController : ControllerBase
    {
        private readonly IImageInfoService? imageInfoService;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ImageInfoController(IImageInfoService? imageInfoService, IWebHostEnvironment webHostEnvironment)
        {
            this.imageInfoService = imageInfoService;
            this.webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("FileUp")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> FileUp(IFormFile file)
        {
            //重命名
            var newName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            //计算hash值
            using(var stream= file.OpenReadStream())
            {
                string hash = HashHelper.ComputeSha256Hash(stream);
                stream.Position = 0;
                var image = await imageInfoService.LoadEntities(i => i.FileHash == hash && i.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
                if (image != null)
                {
                    //说明已经上传过了,直接返回文件路径，实现秒传
                    return Ok(new ApiResult<string> { Code = 200, Message = "更新头像成功", Data = image.ImageUrl, Success = true });
                }
                //构建文件存储路径
                var dir = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot", "images", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), hash);
                Directory.CreateDirectory(dir);
                var fullPath = Path.Combine(dir, newName);
                using(var fileStream=new FileStream(fullPath,FileMode.Create,FileAccess.Write))
                {
                    //写入数据
                     await stream.CopyToAsync(fileStream);
                }
                var imageUrl = Path.Combine("images", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), hash, newName);
                ImageInfo imageInfo = new ImageInfo()
                {
                    ImageUrl = imageUrl,
                    FileHash = hash,
                    IsDeleted = Convert.ToBoolean(DelFlagEnum.Nomal),
                    CreateDate=DateTime.Now
                };
                bool v = await imageInfoService.AddEntity(imageInfo);
                if(v)
                {
                    return Ok(new ApiResult<string> { Code = 200, Message = "更新头像成功", Data = imageUrl, Success = true });
                }
                else
                {
                    return BadRequest(new ApiResult<string> { Code = 400, Message = "更新头像失败", Data = null, Success = false });
                }
            }
            
            
        }
    }
}
