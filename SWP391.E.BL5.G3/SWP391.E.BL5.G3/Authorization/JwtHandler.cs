
using Microsoft.IdentityModel.Tokens;

namespace SWP391.E.BL5.G3.Authorization
{
    public class JwtHandler : DelegatingHandler
    {
        private readonly HttpContextAccessor _contextAccessor;

        public JwtHandler(HttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _contextAccessor.HttpContext?.Request.Cookies["accessToken"];
            if (!token.IsNullOrEmpty())
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return base.Send(request, cancellationToken);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken);
        }
    }
}
