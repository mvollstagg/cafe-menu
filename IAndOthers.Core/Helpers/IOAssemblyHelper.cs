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

            // Get all loaded assemblies in the current AppDomain
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Select all types from all assemblies that are assignable to T
            var types = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => targetType.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                .ToList();

            return types;
        }
    }
}
