using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Fox.Settings;
using API.Fox.EndPoint;
using API.Fox.Modules;

namespace API.Fox.AppBuilder;

internal static class Auth
{
    internal static WebApplicationBuilder AddAppAuth(this WebApplicationBuilder builder, Security security)
    {
        builder.Services.AddAuthentication(o => {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o => {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuers = security.TokenIssuers,
                ValidAudiences = security.TokenAudiences,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(security.SymetricKey)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });
        
        builder.Services.AddAuthorization(options =>
        {
            foreach (var policyClaim in builder.GetEndPointsPolicies())
            {
                options.AddPolicy(policyClaim,
                                  policy => policy.RequireClaim("SystemPermission",
                                                                new string[] { policyClaim }));
            }
        });
        
        return builder;
    }

    internal static IEnumerable<string> GetEndPointsPolicies(this WebApplicationBuilder builder)
    {
        Type endpointInterface = typeof(IEndPoint);

        IEnumerable<Type> endPointsImplementation =
                ModuleReferences.GetAssemblies()
                         .SelectMany(a => a.GetTypes())
                         .Where(c => endpointInterface.IsAssignableFrom(c)
                                     && c.GetConstructor(Type.EmptyTypes) != null);

        List<string> claims = new List<string>();
        foreach (var implementation in endPointsImplementation)
        {
            IEndPoint? implementationInstance = Activator.CreateInstance(implementation) as IEndPoint;
            if (implementationInstance != null)
                claims.Add(implementationInstance.PermissionClaim);
        }
        return claims;
    }
}
