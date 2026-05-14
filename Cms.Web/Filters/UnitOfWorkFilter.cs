using Cms.EntityFrameworkCore;
using Cms.Web.Attributes;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace Cms.Web.Filters
{
    public class UnitOfWorkFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

          var result = await next();
            // 该条件成立，表明请求的控制器方法中的代码，出现异常。
            if (result.Exception != null)
            {
                return;
            }
          var controllerActionDes =  context.ActionDescriptor as ControllerActionDescriptor;
            if(controllerActionDes == null)
            {
                return;
            }
          var unitAttr =  controllerActionDes.MethodInfo.GetCustomAttribute(typeof(UnitOfWorkAttribute));
            if(unitAttr == null)
            {
                return;
            }
           var dbContext = context.HttpContext.RequestServices.GetService<MyDbContext>();
            if (dbContext != null)
            {
                 await dbContext.SaveChangesAsync();
            }
        }
    }
}
