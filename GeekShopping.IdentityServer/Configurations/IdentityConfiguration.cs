using System.Collections.Generic;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace GeekShopping.IdentityServer.Configurations;

public class IdentityConfiguration
{
    private readonly string _webAppUrl;
    private readonly IConfiguration _configuration;

    public IdentityConfiguration(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _webAppUrl = environment.IsDevelopment()
                    ? configuration["WebAppUrlLocalHost"]
                    : configuration["WebAppUrl"];
    }

    public IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource> {
            new IdentityResources.OpenId(),
            new IdentityResources.Email(),
            new IdentityResources.Profile()
        };

    public IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope> {
            new(_configuration["ApiScopeName"], _configuration["ApiScopeDisplayName"]),
            new(name: "read", "Read data."),
            new(name: "write", "Write data."),
            new(name: "delete", "Delete data.")
        };

    public IEnumerable<Client> Clients =>
        new List<Client> {
            new Client
            {
                ClientId = "client",
                ClientSecrets = { new(_configuration["ClientSecrets"].Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "read", "write", "profile" }
            },
            new Client
            {
                ClientId = _configuration["WebAppClientId"],
                ClientSecrets = { new(_configuration["ClientSecrets"].Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = {$"{_webAppUrl}/signin-oidc"},
                PostLogoutRedirectUris = {$"{_webAppUrl}/signout-callback-oidc"},
                AllowedScopes = {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    _configuration["ApiScopeName"]
                }
            },
        };
}
