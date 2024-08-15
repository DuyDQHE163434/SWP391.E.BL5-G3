using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SWP391.E.BL5.G3.Enum;
using System.Security.Claims;

namespace SWP391.E.BL5.G3.Extensions
{
    public class RoleFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var roleClaim = httpContext.User.FindFirst(ClaimTypes.Role)?.Value;

                if (int.TryParse(roleClaim, out int roleValue))
                {
                    var roleEnum = (RoleEnum)roleValue;
                    var roleName = roleEnum.ToString();
                    if( context.Controller is Controller controller)
                    {
                        controller.ViewData["Role"] = roleName;
                    }
                }
                else
                {
                    if (context.Controller is Controller controller)
                    {
                        controller.ViewData["Role"] = "Guest";
                    }
                }
            }
            base.OnActionExecuting(context);
        }
    }
}
