using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Fox.Settings;
using API.Fox.EndPoint;
using Fox.Access.Model;
using System.Linq;
using Fox.Access.Service;

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
            if (implementationInstance != null && !claims.Contains(implementationInstance.PermissionClaim))
                claims.Add(implementationInstance.PermissionClaim);
        }
        return claims;
    }

    public static void CreateAdminUser(this WebApplication app)
    {
        using (var serviceScope = app.Services.CreateScope())
        {
            var userService = serviceScope.ServiceProvider.GetService<UserService>();
            var security = serviceScope.ServiceProvider.GetService<Security>();

            if (userService == null || security == null)
                return;

            User? user = userService.GetUser(security.AdminUserLogin);
            if (user == null) //user doesn't exist
            {
                user = new User()
                {
                    Login = security.AdminUserLogin,
                    Name = security.AdminUserName,
                    Email = security.AdminUserEmail
                };

                try
                {
                    user = userService.CreateAdminUser(user, security.AdminUserPassword);
                }
                catch
                {
                    user = userService.GetUser(security.AdminUserLogin);
                    if (user == null)
                        throw;
                }
            }
        }
    }
}
