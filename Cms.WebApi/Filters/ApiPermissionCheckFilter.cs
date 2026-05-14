using Gms.IService;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Gms.WebApi.Filters
{
    //////////////未来迭代实现，暂时废弃
    //public class ApiPermissionCheckFilter : IAsyncActionFilter
    //{
    //    private readonly IUserInfoService userInfoService;

    //    public ApiPermissionCheckFilter(IUserInfoService userInfoService)
    //    {
    //        this.userInfoService = userInfoService;
    //    }

    //    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    //    {
    //        var url = context.HttpContext.Request.Path; // 获取用户请求的接口地址
    //        var method = context.HttpContext.Request.Method; // 获取接口请求的方式




    //        // 这里通过HttpContext接收前端发送过来的请求头中的Authorization信息。
    //        // 我们知道Authorization中保存了token信息，而token信息中含有用户的编号，这里根据用户的编号查询用户，然后查询用户具有的角色，在通过角色查询用户具有的api权限，查询出用户具有的api权限以后，判断用户当前请求的api接口是否在用户所具有的api权限中，如果在则允许访问，否则给出相应的错误提示。
    //        var authorization = context.HttpContext.Request.Headers["Authorization"];
    //        if (authorization.Count > 0)
    //        {
    //            // 这里需要注意的是，我们前面约定了token信息中包含Bearer （这里是空格）前缀，所以这里需要进行分割，获取真正的token数据。
    //            var token = authorization[0]!.ToString().Split("Bearer ");
    //            // 这里读取token信息（可以打上断点，查看具体的值）
    //            var tokenInfo = new JwtSecurityTokenHandler().ReadJwtToken(token[1]);
    //            // Claims中存储的第一个就是用户的编号
    //            var info = tokenInfo.Claims.FirstOrDefault();
    //            var userInfo = await userInfoService.LoadEntities(u => u.Id.ToString() == info!.Value).Include(u => u.RoleInfos)!.ThenInclude(u => u.PermissionInfos).ToListAsync();
    //            var userPermissionApis = userInfo.SelectMany(u => u.RoleInfos).SelectMany(u => u.PermissionInfos).Where(u => u.PermissionType == 3).ToList();
    //            // 根据用户查询角色，角色找权限，权限表找api权限。List

    //            // 根据请求的地址和请求的方式，查询api权限表，可以找到具体的记录，然后看一下该记录是否在上面的list集合中存在，如果存在，放行。


    //        }
    //        else
    //        {

    //        }


    //        await next();
    //    }
    //}
}
