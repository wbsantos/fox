using System;
using API.Fox.Modules;
using API.Fox.Settings;
using DB.Fox;
using Microsoft.Extensions.DependencyInjection;

namespace API.Fox.AppBuilder;

internal static class RepositoryBuilder
{
	internal static WebApplicationBuilder AddAppRepositories(this WebApplicationBuilder builder)
    {
        DBSettings dbSettings = new();
        builder.Configuration.GetSection("DBSettings").Bind(dbSettings);
        builder.Services.AddSingleton<DBSettings>(dbSettings);

        Type repoInterface = typeof(IRepository);
        IEnumerable<Type> repoImplementation =
                ModuleReferences.GetAssemblies()
                         .SelectMany(a => a.GetTypes())
                         .Where(c => repoInterface.IsAssignableFrom(c)
                                     && !c.IsInterface);
        
        foreach (var implementation in repoImplementation)
        {
            builder.Services.AddTransient(implementation);
        }
        return builder;
    }
}

