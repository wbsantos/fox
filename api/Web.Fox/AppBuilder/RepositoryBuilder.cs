using System;
using System.Data.Common;
using API.Fox.Settings;
using DB.Fox;
using Fox.Access.Repository;
using Fox.Access.Model;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Fox.AppBuilder;

internal static class RepositoryBuilder
{
	internal static WebApplicationBuilder AddAppRepositories(this WebApplicationBuilder builder, Security security, AppInfo appInfo)
    {
        DBSettings dbSettings = new();
        builder.Configuration.GetSection("DBSettings").Bind(dbSettings);
        builder.Services.AddSingleton<DBSettings>(dbSettings);
        DBConnection.Initialize(dbSettings,
                                dbSettings.AutoCreateProcedures,
                                GetTypesThatImplementInterface(typeof(IDBCustomType)));

        IEnumerable<Type> repoImplementation = GetTypesThatImplementInterface(typeof(IRepository));

        builder.Services.AddTransient<DBConnection>();
        foreach (var implementation in repoImplementation)
        {
            builder.Services.AddTransient(implementation);
        }

        CreateAdminUser(security, dbSettings, appInfo);
        return builder;
    }

    private static IEnumerable<Type> GetTypesThatImplementInterface(Type interfaceType)
    {
        return ModuleReferences.GetAssemblies()
                         .SelectMany(a => a.GetTypes())
                         .Where(c => interfaceType.IsAssignableFrom(c)
                                     && !c.IsInterface);
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

