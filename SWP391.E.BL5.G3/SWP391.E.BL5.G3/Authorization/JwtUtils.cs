using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SWP391.E.BL5.G3.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SWP391.E.BL5.G3.Authorization
{
    public class JwtUtils
    {
        private readonly IConfiguration configuration;
        private readonly traveltestContext context;
        public JwtUtils(IConfiguration configuration, traveltestContext context)
        {
            this.configuration = configuration;
            this.context = context; 
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Role, user.RoleId.ToString(),ClaimValueTypes.String, configuration["TokenBearer:Issuer"])
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenBearer:SignatureKey"]));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var infor = new JwtSecurityToken(
                    issuer: configuration["TokenBearer:Issuer"],
                    claims: claims,
                    audience: configuration["TokenBearer:Audience"],
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: credential
                );
            string token = new JwtSecurityTokenHandler().WriteToken(infor);
            return token;
        }

        public int? ValidateToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configuration["TokenBearer:SignatureKey"]);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = Int32.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.Role).Value);

                // return user id from JWT token if validation successful
                return userId;
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }
    }
}
