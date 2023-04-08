using System;
using System.Data.Common;
using API.Fox.EndPoint;
using API.Fox.Settings;
using DB.Fox;
using Fox.Access.Repository;
using Fox.Access.Model;
using Microsoft.Extensions.DependencyInjection;

namespace API.Fox.AppBuilder;

internal static class RepositoryBuilder
{
	internal static WebApplicationBuilder AddAppRepositories(this WebApplicationBuilder builder, Security security, AppInfo appInfo)
    {
        DBSettings dbSettings = new();
        builder.Configuration.GetSection("DBSettings").Bind(dbSettings);
        builder.Services.AddSingleton<DBSettings>(dbSettings);
        if (dbSettings.AutoCreateProcedures)
            new DBConnection(dbSettings).CreateProcedures();

        Type repoInterface = typeof(IRepository);
        IEnumerable<Type> repoImplementation =
                ModuleReferences.GetAssemblies()
                         .SelectMany(a => a.GetTypes())
                         .Where(c => repoInterface.IsAssignableFrom(c)
                                     && !c.IsInterface);

        builder.Services.AddTransient<DBConnection>();
        foreach (var implementation in repoImplementation)
        {
            builder.Services.AddTransient(implementation);
        }

        CreateAdminUser(security, dbSettings, appInfo);
        return builder;
    }

    public static void CreateAdminUser(Security security, DB.Fox.DBSettings dbSettings, AppInfo appInfo)
    {
        DB.Fox.DBConnection dbConnection = new DB.Fox.DBConnection(dbSettings);
        StampRepository stampRepo = new StampRepository(dbConnection, appInfo, new LoggedUser());
        PermissionRepository permissionRepo = new PermissionRepository(dbConnection, stampRepo);
        UserRepository userRepo = new UserRepository(dbConnection, permissionRepo);

        User? user = userRepo.GetUser(security.AdminUserLogin);
        if(user == null) //user doesn't exist
        {
            user = new User()
            {
                Login = security.AdminUserLogin,
                Name = security.AdminUserName,
                Email = security.AdminUserEmail
            };

            try
            {
                user = userRepo.CreateAdminUser(user, security.AdminUserPassword);
            }
            catch
            {
                user = userRepo.GetUser(security.AdminUserLogin);
                if (user == null)
                    throw;
            }
        }
    }
}

