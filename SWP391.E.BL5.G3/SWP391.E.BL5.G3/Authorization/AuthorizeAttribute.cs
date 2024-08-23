using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SWP391.E.BL5.G3.DTOs;
using SWP391.E.BL5.G3.Enum;
using SWP391.E.BL5.G3.Models;

namespace SWP391.E.BL5.G3.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IList<RoleEnum> _roles;
        private readonly traveltestContext _context = new traveltestContext();

        public AuthorizeAttribute(params RoleEnum[] roles)
        {
            //_roles = roles ?? new Role[] { };
            _roles = roles ?? new RoleEnum[] { };
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // skip authorization if action is decorated with [AllowAnonymous] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            // authorization
            var user = (User)context.HttpContext.Items["User"];
            try
            {
                if (user == null)
                {
                    // not logged in or role not authorized
                    context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                    context.Result = new RedirectResult("/Error/Error401", false);
                    return;
                }
                else
                {
                    var userDTO = toDTO(user);
                    if (user == null || (_roles.Any() && !_roles.Any(item => userDTO.Roles.Equals(item))))
                    {
                        // not logged in or role not authorized
                        context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                    }
                }
            }
            catch (Exception ex)
            {
                context.Result = new RedirectResult("/Error/Error401", false);
            }

        }

        public UserDTO toDTO(User user)
        {
            var userDTO = new UserDTO();
            userDTO.UserId = user.UserId;
            userDTO.Email = user.Email;
            userDTO.FirstName = user.FirstName;
            userDTO.LastName = user.LastName;

            if (user.RoleId == 1)
            {
                userDTO.Roles = RoleEnum.Admin;
            }
            else if (user.RoleId == 2)
            {
                userDTO.Roles = RoleEnum.Travel_Agent;
            }
            else if (user.RoleId == 3)
            {
                userDTO.Roles = RoleEnum.Customer;
            }

            return userDTO;
        }
    }
}