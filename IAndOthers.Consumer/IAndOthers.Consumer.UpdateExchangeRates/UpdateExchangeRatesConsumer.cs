using IAndOthers.Infrastructure.Data;
using IAndOthers.Core.Consumer;
using MassTransit;
using IAndOthers.Core.Data.Services;
using IAndOthers.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using IAndOthers.Application.Job;
using IAndOthers.Core.IoC;

namespace IAndOthers.Consumer.UpdateExchangeRates
{
    public class UpdateExchangeRatesConsumer : IConsumer<UpdateExchangeRatesMessage>, IIOConsumer
    {
        private readonly IIORepository<AuthGuestUser, ApplicationDbContext> _repository;

        public string QueueName => "update_exchange_rates_queue";

        public UpdateExchangeRatesConsumer()
        {
            _repository = IODependencyResolver.Resolve<IIORepository<AuthGuestUser, ApplicationDbContext>>();
        }

        public async Task Consume(ConsumeContext<UpdateExchangeRatesMessage> context)
        {
            try
            {
                // Do something
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.UtcNow}] - An error occurred: {ex.Message}");
            }
        }
    }
}
