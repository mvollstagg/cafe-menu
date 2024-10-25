using IAndOthers.Core.Configs;
using IAndOthers.Core.Consumer;
using IAndOthers.Core.IoC;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace IAndOthers.Core.MassTransit
{
    public class MassTransitBusHelper : IIODependencySingleton
    {
        private readonly RabbitMqConfig _rabbitMqConfig;
        private IBusControl _busControl;  // Store the IBusControl instance

        public MassTransitBusHelper(IOptions<RabbitMqConfig> rabbitMqConfig)
        {
            _rabbitMqConfig = rabbitMqConfig.Value;
        }

        public void ConfigureMassTransit(IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                var consumers = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => typeof(IIOConsumer).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

                foreach (var consumerType in consumers)
                {
                    x.AddConsumer(consumerType);  // Register the consumer
                }

                // Configure RabbitMQ with bus control
                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMqConfig = context.GetRequiredService<IOptions<RabbitMqConfig>>().Value;

                    cfg.Host(rabbitMqConfig.Host, rabbitMqConfig.VirtualHost, h =>
                    {
                        h.Username(rabbitMqConfig.UserName);
                        h.Password(rabbitMqConfig.Password);
                    });

                    // Configure each consumer endpoint
                    foreach (var consumerType in consumers)
                    {
                        var consumerInstance = (IIOConsumer)Activator.CreateInstance(consumerType);
                        var queueName = consumerInstance.QueueName;

                        cfg.ReceiveEndpoint(queueName, e =>
                        {
                            e.ConfigureConsumer(context, consumerType);  // DI resolves consumers
                        });
                    }
                });
            });

            services.AddSingleton(provider =>
            {
                _busControl = provider.GetRequiredService<IBusControl>();  // Initialize IBusControl singleton
                return _busControl;
            });
        }

        // Start the bus control manually
        public async Task StartBusAsync()
        {
            if (_busControl != null)
            {
                await _busControl.StartAsync();  // Start the bus
            }
        }

        // Stop the bus control manually (for graceful shutdown)
        public async Task StopBusAsync()
        {
            if (_busControl != null)
            {
                await _busControl.StopAsync();  // Stop the bus to avoid memory leaks
            }
        }
    }
}
