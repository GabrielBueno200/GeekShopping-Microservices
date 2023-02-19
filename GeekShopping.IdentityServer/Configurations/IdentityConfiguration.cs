using System.Collections.Generic;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Microsoft.Extensions.Configuration;

namespace GeekShopping.IdentityServer.Configurations;

public class IdentityConfiguration
{
    public const string Admin = "Admin";
    public const string Client = "Client";
    private readonly string _webAppUrl;

    public IdentityConfiguration(IConfiguration configuration)
    {
        _webAppUrl = configuration["WebAppUrl"]!;
    }
    
    public IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource> {
            new IdentityResources.OpenId(),
            new IdentityResources.Email(),
            new IdentityResources.Profile()
        };

    public IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope> {
            new("geek_shopping", "GeekShopping Server"),
            new(name: "read", "Read data."),
            new(name: "write", "Write data."),
            new(name: "delete", "Delete data.")
        };

    public IEnumerable<Client> Clients =>
        new List<Client> {
            new Client 
            { 
                ClientId = "client",
                ClientSecrets = { new("my_super_secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "read", "write", "profile" }
            },
            new Client 
            { 
                ClientId = "geek_shopping",
                ClientSecrets = { new("my_super_secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = {$"{_webAppUrl}/signin-oidc"},
                PostLogoutRedirectUris = {$"{_webAppUrl}/signout-callback-oidc"},
                AllowedScopes = { 
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "geek_shopping"
                }
            },
        };
}
