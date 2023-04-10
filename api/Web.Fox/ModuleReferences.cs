using System;
using System.Collections.Generic;
using System.Reflection;
using Web.Fox.Pages.Menu;

namespace Web.Fox;

public static class ModuleReferences
{
	internal static IEnumerable<Assembly> GetAssemblies()
	{
        var returnAssemblies = new List<Assembly>();
        var loadedAssemblies = new HashSet<string>();
        var assembliesToCheck = new Queue<Assembly>();

        Assembly? entryDll = Assembly.GetExecutingAssembly();
        if (entryDll == null)
            return returnAssemblies;

        assembliesToCheck.Enqueue(entryDll);
        returnAssemblies.Add(entryDll);

        while (assembliesToCheck.Any())
        {
            var assemblyToCheck = assembliesToCheck.Dequeue();
            
            foreach (var reference in assemblyToCheck.GetReferencedAssemblies()
                                                     .Where(x => x.Name != null
                                                                 && !x.Name.StartsWith("Swashbuckle.")
                                                                 && !x.Name.StartsWith("Microsoft.")
                                                                 && !x.Name.StartsWith("System.")))
            {
                if (!loadedAssemblies.Contains(reference.FullName))
                {
                    var assembly = Assembly.Load(reference);
                    assembliesToCheck.Enqueue(assembly);
                    loadedAssemblies.Add(reference.FullName);
                    returnAssemblies.Add(assembly);
                }
            }
        }

        return returnAssemblies;        
    }

    private static IEnumerable<INavBarItem>? _menuPages = null;
    public static IEnumerable<INavBarItem> GetMenuPages(IServiceProvider serviceProvider)
    {
        if (_menuPages != null)
            return _menuPages;

        IList<INavBarItem> menuPages = new List<INavBarItem>();
        Type menuPageType = typeof(Web.Fox.Pages.Menu.INavBarItem);
        IEnumerable<Type> typesMenuPage =
                Web.Fox.ModuleReferences.GetAssemblies()
                                        .SelectMany(a => a.GetTypes())
                                        .Where(c => menuPageType.IsAssignableFrom(c)
                                                    && !c.IsInterface);
        foreach (var item in typesMenuPage)
        {
            var instance = ActivatorUtilities.CreateInstance(serviceProvider, item) as INavBarItem;
            if (instance != null)
                menuPages.Add(instance);
        }
        _menuPages = menuPages;
        return menuPages;
    }

    public static void HasPermission(this HttpContext httpContext, string claimToTest)
    {
        bool permission = httpContext?.User
                                     ?.Claims
                                     ?.Any(claim => claim.Type == "SystemPermission" &&
                                                    (claim.Value == claimToTest || claim.Value == "admin"))
                          ?? false;
        if (!permission)
            throw new UnauthorizedAccessException("User has no permission to perform this action");
    }
}

