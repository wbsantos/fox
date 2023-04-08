using System;
using System.Collections.Generic;
using System.Reflection;
using Web.Fox.Pages.Menu;

namespace Web.Fox;

internal static class ModuleReferences
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
    internal static IEnumerable<INavBarItem> GetMenuPages()
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
            var instance = Activator.CreateInstance(item) as Web.Fox.Pages.Menu.INavBarItem;
            if (instance != null)
                menuPages.Add(instance);
        }
        _menuPages = menuPages;
        return menuPages;
    }
}

