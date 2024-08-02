using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Security.Cryptography;

namespace send.api.Shared.Authentication.Config
{
    public class ConfigurationBuilder
    {
        private IServiceCollection _services;
        public ConfigurationBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public void AddAuth(IAuthentication<JwtBearerOptions> authentication, string publicKeyXML, string issuer)
        {
            authentication.AddAuth(_services, options =>
            {
                RsaSecurityKey issuerSigningKey = null;

                RSA publicRsa = RSA.Create();

                string publicKeyXml = publicKeyXML;

                publicRsa.FromXmlStringCustom(publicKeyXml);
                issuerSigningKey = new RsaSecurityKey(publicRsa);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidIssuer = issuer,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = issuerSigningKey,
                    RequireSignedTokens = true,
                    ClockSkew = TimeSpan.FromHours(1),

                };

                options.RequireHttpsMetadata = false;

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.ForegroundColor = ConsoleColor.Red;

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = ctx =>
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;


                        var identity = ctx.Principal.Identity;
                        var username = ctx.Principal.Claims.FirstOrDefault(c => c.Type == "preferred_username").Value;

                        var clientRoles = ctx.Principal.Claims.FirstOrDefault(c => c.Type == "resource_access") != null ?
                            ctx.Principal.Claims.FirstOrDefault(c => c.Type == "resource_access").Value : null;

                        if (clientRoles != null)
                        {
                            string resourceAccess = Environment.GetEnvironmentVariable("RESOURCE_ACCESS") ?? "";
                            var resourceKeyList = resourceAccess.Contains(',') ? resourceAccess.Split(',') : new string[] { resourceAccess };
                            var roles = JsonConvert.DeserializeObject<RolesObject>(SearchResources(clientRoles, resourceKeyList));

                            var claims = new List<Claim>();

                            claims.Add(new Claim(ClaimTypes.Name, username));

                            if (roles != null)
                            {
                                foreach (var r in roles.Roles)
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, r));
                                }

                                var appIdentity = new ClaimsIdentity(claims);

                                ctx.Principal.AddIdentity(appIdentity);
                            }
                        }

                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.ForegroundColor = ConsoleColor.Red;

                        var d = context;
                        Console.WriteLine(DateTime.Now);

                        return Task.CompletedTask;
                    }
                };
            });
        }

        private string SearchResources(string clientRole, string[] resouceKeyList)
        {
            JObject resourceAccessObject = JObject.Parse(clientRole);

            foreach (var resourcekey in resouceKeyList)
            {
                foreach (var resourceObj in resourceAccessObject)
                {
                    if (resourceObj.Key.ToLower() == resourcekey.Trim().ToLower())
                    {
                        JToken value = resourceObj.Value;
                        return value.ToString();
                    }
                }
            }

            return string.Empty;
        }
    }

    public class RolesObject
    {
        public string[] Roles { get; set; }
    }
    public class RealmAccess
    {
        public List<string> Roles { get; set; }
    }
}
