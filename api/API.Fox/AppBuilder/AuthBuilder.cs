using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Fox.Settings;

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
        
        builder.Services.AddAuthorization();
        
        return builder;
    }
}
