using System;
using GeekShopping.Web.Services;
using GeekShopping.Web.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Logging;
using GeekShopping.IoC.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
IdentityModelEventSource.ShowPII = true;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
    .AddCookie("Cookies", options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    })
    .AddOpenIdConnect("oidc", options =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // It is important this matches the actual URL of your identity server, not the Docker internal URL
            options.Authority = $"{builder.Configuration["ServicesUrls:IdentityServerLocalHost"]}";

            // This will allow the container to reach the discovery endpoint
            options.MetadataAddress = $"{builder.Configuration["ServicesUrls:IdentityServer"]}/.well-known/openid-configuration";
            options.RequireHttpsMetadata = false;
            options.Events.OnRedirectToIdentityProvider = context =>
            {
                // Intercept the redirection so the browser navigates to the right URL in your host
                context.ProtocolMessage.IssuerAddress = $"{builder.Configuration["ServicesUrls:IdentityServerLocalHost"]}/connect/authorize";
                return Task.CompletedTask;
            };
        }
        else
            options.Authority = builder.Configuration["ServicesUrls:IdentityServer"];
        options.GetClaimsFromUserInfoEndpoint = true;
        options.ClientId = builder.Configuration["WebAppClientId"];
        options.ClientSecret = builder.Configuration["ClientSecrets"];
        options.ResponseType = builder.Configuration["ResponseType"];
        options.ClaimActions.MapJsonKey("role", "role", "role");
        options.ClaimActions.MapJsonKey("sub", "sub", "sub");
        options.TokenValidationParameters.NameClaimType = "name";
        options.TokenValidationParameters.RoleClaimType = "role";
        options.Scope.Add(builder.Configuration["ApiScopeName"]);
        options.SaveTokens = true;
    });

builder.Services.AddHttpContextAccessor();

builder.Services.AddSignedHttpClient<IProductService, ProductService>(
    baseUrl: builder.Configuration["ServicesUrls:ProductApi"]
);

builder.Services.AddSignedHttpClient<ICartService, CartService>(
    baseUrl: builder.Configuration["ServicesUrls:CartApi"]
);

builder.Services.AddSignedHttpClient<ICouponService, CouponService>(
    baseUrl: builder.Configuration["ServicesUrls:CouponApi"]
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
