using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IAndOthers.Core.Configs;
using System.Reflection;

namespace IAndOthers.Core.MassTransit
{
    public static class MassTransitHelper
    {
        private static IBusControl _busControl;

        /// <summary>
        /// Configures the bus control using the service provider.
        /// </summary>
        public static void ConfigureBusControl(IServiceProvider serviceProvider)
        {
            _busControl = serviceProvider.GetRequiredService<IBusControl>();
        }

        /// <summary>
        /// Adds a queue for the specified message type and consumer asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">The message type that the queue consumes.</typeparam>
        /// <param name="message">The message to be sent.</param>
        /// <param name="queueName">The queue name to send the message to.</param>
        public static async Task AddQueueAsync<TMessage>(TMessage message, string queueName)
            where TMessage : class
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(config =>
            {
                config.Host(new Uri("rabbitmq://" + "localhost"), h =>
                {
                    h.Username("admin");
                    h.Password("vBWwT9Kt3rEnZ8dQVMYxPS");
                });

                config.ReceiveEndpoint(queueName, e =>
                {
                    e.Durable = true;
                });
            });

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await busControl.StartAsync(source.Token);

            var endPoint = await busControl.GetSendEndpoint(new Uri("queue:" + queueName));
            await endPoint.Send(message);

            await busControl.StopAsync();
        }
    }

}
