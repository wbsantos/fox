using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Fox.AppBuilder;

internal static class Auth
{
    internal static WebApplicationBuilder AddFoxAuth(this WebApplicationBuilder builder)
    {
        //TODO the data below should be loaded from config files or environment variables
        builder.Services.AddAuthentication(o => {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o => {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = "fox.app",
                ValidAudience = "fox.app",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("wbsantosabcdefghijklmnopq")),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });
        
        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();

        return builder;
    }
}
