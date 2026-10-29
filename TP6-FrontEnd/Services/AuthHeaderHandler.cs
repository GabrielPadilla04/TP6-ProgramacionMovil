using System.Net.Http.Headers;

namespace TP6_FrontEnd.Services
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly IAuthService _authService;

        public AuthHeaderHandler(IAuthService authService)
        {
            _authService = authService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _authService.ObtenerTokenAsync();

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}