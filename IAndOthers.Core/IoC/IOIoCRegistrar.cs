using Microsoft.Extensions.DependencyInjection;

namespace IAndOthers.Core.IoC
{
    public static class IOIoCRegistrar
    {
        public static void RegisterDependencies(IServiceCollection services)
        {
            // Get all assemblies in the current domain
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Iterate through each assembly
            foreach (var assembly in assemblies)
            {
                // Get all types in the assembly
                var types = assembly.GetTypes()
                    .Where(type => type.IsClass && !type.IsAbstract) // Filter for non-abstract classes
                    .ToList();

                foreach (var type in types)
                {
                    // Get all interfaces implemented by the type
                    var interfaces = type.GetInterfaces();

                    foreach (var @interface in interfaces)
                    {
                        if (@interface.IsGenericType)
                        {
                            var genericTypeDefinition = @interface.GetGenericTypeDefinition();

                            if (genericTypeDefinition == typeof(IIODependencyTransient<>))
                            {
                                // Register as Transient with interface
                                services.AddTransient(@interface.GetGenericArguments()[0], type);
                            }
                            else if (genericTypeDefinition == typeof(IIODependencyScoped<>))
                            {
                                // Register as Scoped with interface
                                services.AddScoped(@interface.GetGenericArguments()[0], type);
                            }
                            else if (genericTypeDefinition == typeof(IIODependencySingleton<>))
                            {
                                // Register as Singleton with interface
                                services.AddSingleton(@interface.GetGenericArguments()[0], type);
                            }
                        }
                        else
                        {
                            if (@interface == typeof(IIODependencyTransient))
                            {
                                // Register as Transient without interface
                                services.AddTransient(type);
                            }
                            else if (@interface == typeof(IIODependencyScoped))
                            {
                                // Register as Scoped without interface
                                services.AddScoped(type);
                            }
                            else if (@interface == typeof(IIODependencySingleton))
                            {
                                // Register as Singleton without interface
                                services.AddSingleton(type);
                            }
                        }
                    }
                }
            }
        }
    }
}