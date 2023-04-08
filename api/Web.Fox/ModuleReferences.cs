using System;
using System.Collections.Generic;
using System.Reflection;

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
}

