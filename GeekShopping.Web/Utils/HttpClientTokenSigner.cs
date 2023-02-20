using System;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace GeekShopping.Web.Utils;

public static class HttpClientTokenSigner 
{
    public static void AddSignedHttpClient<TInterface, TEntity>(this IServiceCollection services, string baseUrl)
        where TInterface : class
        where TEntity : class, TInterface
    {
        var httpContextAccessor = services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>();

        services.AddHttpClient<TInterface, TEntity>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);

            var token = httpContextAccessor.HttpContext?.GetTokenAsync("access_token").GetAwaiter().GetResult();
            
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        });
    }
}
