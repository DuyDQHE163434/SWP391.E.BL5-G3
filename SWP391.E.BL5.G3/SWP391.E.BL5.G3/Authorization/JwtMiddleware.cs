namespace SWP391.E.BL5.G3.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {   
            _next = next;
        }

        public async Task Invoke(HttpContext context, UserService userService, JwtUtils jwtUtils)
        {
            string token;
            token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            //use for view
            if (token == null || string.IsNullOrEmpty(token))
            {
                if (context.Request.Cookies["accessToken"] != null)
                {
                    token = context.Request.Cookies["accessToken"];
                }
            }
            var userId = jwtUtils.ValidateToken(token);
            if (userId != null)
            {
                // attach user to context on successful jwt validation
                context.Items["User"] = userService.GetUserById(userId.Value);
            }
            await _next(context);
        }
    }
}
