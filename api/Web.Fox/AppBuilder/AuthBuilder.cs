using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Fox.Settings;
using Fox.Access.Model;
using System.Linq;

namespace Web.Fox.AppBuilder;

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
                                                                new string[] { policyClaim, "admin" }));
            }
        });

        builder.Services.AddScoped<LoggedUser>(provider =>
        {
            var httpContext = provider.GetRequiredService<IHttpContextAccessor>();
            Func<string, string> getClaim = key => httpContext?.HttpContext
                                                              ?.User
                                                              ?.Claims
                                                              ?.FirstOrDefault(c => c.Type == key)
                                                              ?.Value ?? string.Empty;
            LoggedUser user = new LoggedUser();
            string userId = getClaim(nameof(User.Id));
            if (string.IsNullOrEmpty(userId))
                return user;
            
            user.Id = new Guid(userId);
            user.Name = getClaim(nameof(User.Name));
            user.Login = getClaim(nameof(User.Login));
            user.Email = getClaim(nameof(User.Email));

            return user;
        });
        return builder;
    }

    internal static IEnumerable<string> GetEndPointsPolicies(this WebApplicationBuilder builder)
    {
        //TODO: return policies being used by the web app
        return Array.Empty<string>();
    }
}
