using SWP391.E.BL5.G3.Enum;

namespace SWP391.E.BL5.G3.DTOs
{
    public static class RoleExtensions
    {
        public static string ToRoleName(this RoleEnum role)
        {
            return role switch
            {
                RoleEnum.Admin => "Admin",
                RoleEnum.Travel_Agent => "Travel_Agent",
                RoleEnum.Customer => "Customer",
                _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
            };
        }
    }
}   

