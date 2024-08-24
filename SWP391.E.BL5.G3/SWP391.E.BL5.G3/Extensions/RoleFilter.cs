using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SWP391.E.BL5.G3.Enum;
using SWP391.E.BL5.G3.Models;
using System.Security.Claims;

namespace SWP391.E.BL5.G3.Extensions
{
    public class RoleFilter : ActionFilterAttribute
    {
        private readonly traveltestContext _context;

        public RoleFilter(traveltestContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var roleClaim = httpContext.User.FindFirst(ClaimTypes.Role)?.Value;
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = _context.Users.FirstOrDefault(x => x.UserId == int.Parse(userId));

                if (int.TryParse(roleClaim, out int roleValue))
                {
                    var roleEnum = (RoleEnum)roleValue;
                    var roleName = roleEnum.ToString();
                    if (context.Controller is Controller controller)
                    {
                        controller.ViewData["Role"] = roleName;
                        controller.ViewData["Avatar"] = user.Image;
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
