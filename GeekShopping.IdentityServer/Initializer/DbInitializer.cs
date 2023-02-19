using System.Security.Claims;
using GeekShopping.IdentityServer.Configurations;
using GeekShopping.IdentityServer.Model;
using GeekShopping.IdentityServer.Model.Context;
using IdentityModel;
using Microsoft.AspNetCore.Identity;

namespace GeekShopping.IdentityServer.Initializer;

public class DbInitializer : IDbInitializer
{
    private readonly MySQLContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DbInitializer(
        MySQLContext context, 
        UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole> roleManager
    )
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public void Initialize()
    {
        if (_roleManager.FindByNameAsync(IdentityConfiguration.Admin).Result is not null) 
            return;

        _roleManager.CreateAsync(new IdentityRole(IdentityConfiguration.Admin)).GetAwaiter().GetResult();
        _roleManager.CreateAsync(new IdentityRole(IdentityConfiguration.Client)).GetAwaiter().GetResult();

        AddAdmin();
        AddClient();
    }

    private void AddAdmin()
    {
        var admin = new ApplicationUser {
            UserName = "user-admin",
            Email = "useradmin@admin.com.br",
            EmailConfirmed = true,
            PhoneNumber = "+55 (11) 12345-6789",
            FirstName = "User",
            LastName = "Admin"
        };

        _userManager.CreateAsync(admin, "Password@123").GetAwaiter().GetResult();
        _userManager.AddToRoleAsync(admin, IdentityConfiguration.Admin).GetAwaiter().GetResult();
        var adminClaimns = _userManager.AddClaimsAsync(admin, new[]
        {
            new Claim(JwtClaimTypes.Name, $"{admin.FirstName} {admin.LastName}"),
            new Claim(JwtClaimTypes.GivenName, admin.FirstName),
            new Claim(JwtClaimTypes.Role, IdentityConfiguration.Admin)
        }).GetAwaiter().GetResult();
    }

    private void AddClient()
    {
        var client = new ApplicationUser {
            UserName = "user-client",
            Email = "userclient@client.com.br",
            EmailConfirmed = true,
            PhoneNumber = "+55 (11) 12345-6789",
            FirstName = "User",
            LastName = "client"
        };

        _userManager.CreateAsync(client, "Password@123").GetAwaiter().GetResult();
        _userManager.AddToRoleAsync(client, IdentityConfiguration.Admin).GetAwaiter().GetResult();
        var adminClaimns = _userManager.AddClaimsAsync(client, new[]
        {
            new Claim(JwtClaimTypes.Name, $"{client.FirstName} {client.LastName}"),
            new Claim(JwtClaimTypes.GivenName, client.FirstName),
            new Claim(JwtClaimTypes.Role, IdentityConfiguration.Client)
        }).GetAwaiter().GetResult();
    }
}
