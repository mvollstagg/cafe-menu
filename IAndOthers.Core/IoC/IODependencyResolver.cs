using Microsoft.Extensions.DependencyInjection;

namespace IAndOthers.Core.IoC
{
    public static class IODependencyResolver
    {
        private static IServiceProvider _serviceProvider;

        public static void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static T Resolve<T>()
        {
            if (_serviceProvider == null)
            {
                throw new InvalidOperationException("ServiceProvider is not set. Call SetServiceProvider during application startup.");
            }

            return _serviceProvider.GetRequiredService<T>();
        }

        public static object Resolve(Type serviceType)
        {
            if (_serviceProvider == null)
            {
                throw new InvalidOperationException("ServiceProvider is not set. Call SetServiceProvider during application startup.");
            }

            return _serviceProvider.GetRequiredService(serviceType);
        }
    }
}