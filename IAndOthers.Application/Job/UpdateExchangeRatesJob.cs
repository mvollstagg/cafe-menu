using IAndOthers.Core.Jobs;
using IAndOthers.Core.MassTransit;
using MassTransit;

namespace IAndOthers.Application.Job
{
    public class UpdateExchangeRatesJob : IORecurringJob
    {
        public override string Code => "UpdateExchangeRates";
        public override string Cron => "0 0 * * *";

        public override async Task Execute()
        {
            Console.WriteLine("[UpdateExchangeRates] Executing the job to remove guest accounts...");

            var message = new UpdateExchangeRatesMessage
            {
                DateUtc = DateTime.UtcNow
            };

            await MassTransitHelper.AddQueueAsync(message, "update_exchange_rates_queue");
            Console.WriteLine("[UpdateExchangeRates] Job completed.");
        }
    }

    public class UpdateExchangeRatesMessage
    {
        public DateTime DateUtc { get; set; }
    }
}
