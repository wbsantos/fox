using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;
using API.Fox.Settings;
using Fox.Access.Model;
using System.Linq;
using Fox.Access.Service;

namespace Web.Fox.AppBuilder;

internal static class Auth
{
    internal static WebApplicationBuilder AddAppAuth(this WebApplicationBuilder builder, Security security)
    {
        builder.Services.AddAuthentication(o => {
            o.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }).AddCookie("Cookies", options =>
        {
            options.LoginPath = "/login";
            options.ExpireTimeSpan = TimeSpan.FromDays(1);
        });
        
        builder.Services.AddAuthorization(options =>
        {
            foreach (var policyClaim in GetEndPointsPolicies())
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

    internal static IEnumerable<string> GetEndPointsPolicies()
    {
        //TODO: it should load the policies dynamically
        return new string[] {
            "USER_SELF_MANAGEMENT",

            "USER_CREATION_MANAGEMENT",
            "USER_DELETION_MANAGEMENT",
            "USER_READ_MANAGEMENT",
            "USER_READ_ALL_MANAGEMENT",
            "USER_MANAGEMENT_READ_GROUP",
            "USER_UPDATE_MANAGEMENT",

            "SYSTEM_PERMISSION_ADDITION",
            "SYSTEM_PERMISSION_REMOVAL",
            "SYSTEM_PERMISSION_READ",

            "GROUP_MANAGEMENT_ADDUSER",
            "GROUP_CREATION_MANAGEMENT",
            "GROUP_DELETION_MANAGEMENT",
            "GROUP_MANAGEMENT_DELUSER",
            "GROUP_READ_MANAGEMENT",
            "GROUP_READ_ALL_MANAGEMENT",
            "GROUP_MANAGEMENT_READ_USER",
            "GROUP_UPDATE_MANAGEMENT",

            "DOCUMENT_PERMISSION_ADDITION",
            "DOCUMENT_CREATION",
            "DOCUMENT_DELETION",
            "DOCUMENT_PERMISSION_REMOVAL",
            "DOCUMENT_READ",
            "DOCUMENT_PERMISSION_READ",
            "DOCUMENT_UPDATE"
        };
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
