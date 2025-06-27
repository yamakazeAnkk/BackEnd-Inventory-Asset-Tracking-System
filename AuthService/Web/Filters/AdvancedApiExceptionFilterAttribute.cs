using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthService.Web.Filters
{
    public class AdvancedApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var filter = context.HttpContext.RequestServices.GetService<AdvancedApiExceptionFilter>();
            filter?.OnException(context);
        }
    }
} 