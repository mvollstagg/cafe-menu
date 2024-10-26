using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IAndOthers.Core.Helpers
{
    public static class IOAssemblyHelper
    {
        public static IEnumerable<Type> FindClassesTypeOf<T>()
        {
            var targetType = typeof(T);
            var types = new List<Type>();

            // Get all loaded assemblies in the current AppDomain
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var assemblyTypes = assembly.GetTypes()
                        .Where(type => targetType.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                        .ToList();

                    types.AddRange(assemblyTypes);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // Log or handle the exception gracefully
                    Console.WriteLine($"Failed to load types from assembly {assembly.FullName}: {ex.Message}");
                    foreach (var loaderException in ex.LoaderExceptions)
                    {
                        Console.WriteLine($"Loader exception: {loaderException.Message}");
                    }
                }
            }

            return types;
        }
    }
}
