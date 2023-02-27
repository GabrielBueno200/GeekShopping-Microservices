using GeekShopping.IoC.Utilities.SharedModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace GeekShopping.IoC.DependencyInjection;

public static class AuthConfigs
{
    public static void AddAuthConfigs(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = configuration["IdentityServerUrl"];
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
            });

        services.AddAuthorization(options =>
            options.AddPolicy("ApiScope", policy =>
            {
                options.AddPolicy(Role.Admin, policy => policy.RequireRole(Role.Admin));
                options.AddPolicy(Role.Client, policy => policy.RequireRole(Role.Client));
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "geek_shopping");
            })
        );
    }
}