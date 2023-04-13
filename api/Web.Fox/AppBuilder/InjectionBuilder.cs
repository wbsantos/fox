using System;
using System.Data.Common;
using API.Fox.Settings;
using DB.Fox;
using Fox.Access.Service;
using Fox.Access.Model;
using Microsoft.Extensions.DependencyInjection;
using Fox.Dox.Storage;

namespace Web.Fox.AppBuilder;

internal static class InjectionBuilder
{
	internal static WebApplicationBuilder AddAppInjections(this WebApplicationBuilder builder, Security security, AppInfo appInfo)
    {
        DBSettings dbSettings = new();
        builder.Configuration.GetSection("DBSettings").Bind(dbSettings);
        builder.Services.AddSingleton<DBSettings>(dbSettings);
        DBConnection.Initialize(dbSettings,
                                dbSettings.AutoCreateProcedures,
                                GetTypesThatImplementInterface(typeof(IDBCustomType)));

        IEnumerable<Type> repoImplementation = GetTypesThatImplementInterface(typeof(IService))
                                              .Union(GetTypesThatImplementInterface(typeof(IRepository)));

        builder.Services.AddTransient<DBConnection>();
        foreach (var implementation in repoImplementation)
        {
            builder.Services.AddTransient(implementation);
        }

        FileStorageSettings fileStorageSettings = new();
        builder.Configuration.GetSection("FileStorage").Bind(fileStorageSettings);
        builder.Services.AddSingleton<FileStorageSettings>(fileStorageSettings);
        builder.Services.AddTransient<IFileStorage, FileStorage>();

        return builder;
    }

    private static IEnumerable<Type> GetTypesThatImplementInterface(Type interfaceType)
    {
        return ModuleReferences.GetAssemblies()
                         .SelectMany(a => a.GetTypes())
                         .Where(c => interfaceType.IsAssignableFrom(c)
                                     && !c.IsInterface);
    }
}

