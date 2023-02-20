using System.Net.Http.Headers;
using System;
using GeekShopping.Web.Services;
using GeekShopping.Web.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using GeekShopping.Web.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(options => {
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
    .AddCookie("Cookies", cookie => cookie.ExpireTimeSpan = TimeSpan.FromMinutes(10))
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = builder.Configuration["ServicesUrls:IdentityServer"];
        options.GetClaimsFromUserInfoEndpoint = true;
        options.ClientId = "geek_shopping";
        options.ClientSecret = "my_super_secret";
        options.ResponseType = "code";
        options.ClaimActions.MapJsonKey("role", "role", "role");
        options.ClaimActions.MapJsonKey("sub", "sub", "sub");
        options.TokenValidationParameters.NameClaimType = "name";
        options.TokenValidationParameters.RoleClaimType = "role";
        options.Scope.Add("geek_shopping");
        options.SaveTokens = true;
    });

builder.Services.AddHttpContextAccessor();
// !

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
