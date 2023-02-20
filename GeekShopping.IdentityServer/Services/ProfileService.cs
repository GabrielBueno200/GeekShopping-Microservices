using System.Linq;
using System.Threading.Tasks;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using GeekShopping.IdentityServer.Model;
using IdentityModel;
using Microsoft.AspNetCore.Identity;

namespace GeekShopping.IdentityServer.Services;

public class ProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;

    public ProfileService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var subject = context.Subject.GetSubjectId();
        var user = await _userManager.FindByIdAsync(subject);
        var userClaims = await _userClaimsPrincipalFactory
            .CreateAsync(user!);

        var claims = userClaims.Claims.ToList();
        claims.Add(new(JwtClaimTypes.FamilyName, user!.LastName));
        claims.Add(new(JwtClaimTypes.GivenName, user.FirstName));

        if (_userManager.SupportsUserRole)
        {
            var roles = await _userManager.GetRolesAsync(user);
            
            foreach (string role in roles)
            {
                claims.Add(new(JwtClaimTypes.Role, role));
                if (_roleManager.SupportsRoleClaims)
                {
                    var identityRole = await _roleManager.FindByNameAsync(role);
                    if (identityRole is not null)
                        claims.AddRange(await _roleManager.GetClaimsAsync(identityRole));
                }
            }
        }
        context.IssuedClaims = claims;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var subject = context.Subject.GetSubjectId();
        var user = await _userManager.FindByIdAsync(subject);
        context.IsActive = user is not null;
    }
}
