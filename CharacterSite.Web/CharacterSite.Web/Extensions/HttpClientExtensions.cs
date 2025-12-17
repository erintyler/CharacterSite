using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CharacterSite.Web.Extensions;

public static class HttpClientExtensions
{
    extension(IHttpClientBuilder builder)
    {
        public IHttpClientBuilder AddAuthToken()
        {
            builder.Services.AddHttpContextAccessor();

            builder.Services.TryAddTransient<HttpClientAuthorizationDelegatingHandler>();
            builder.AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();

            return builder;
        }
    }

    private class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpClientAuthorizationDelegatingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public HttpClientAuthorizationDelegatingHandler(
            IHttpContextAccessor httpContextAccessor,
            HttpMessageHandler innerHandler) : base(innerHandler)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;

            if (context is null) return await base.SendAsync(request, cancellationToken);

            var accessToken = await context.GetTokenAsync("access_token");

            if (!string.IsNullOrWhiteSpace(accessToken))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}