using IAndOthers.Core.Jobs;
using IAndOthers.Core.MassTransit;
using MassTransit;

namespace IAndOthers.Application.Job
{
    public class UpdateExchangeRatesJob : IORecurringJob
    {
        public override string Code => "UpdateExchangeRates";
        public override string Cron => "0 0 * * *"; // Runs every day at midnight

        public override async Task Execute()
        {
            // Your RabbitMQ message logic
            Console.WriteLine("[UpdateExchangeRates] Executing the job to remove guest accounts...");

            // Prepare the message to send to RabbitMQ
            var message = new DeleteGuestAccountsMessage
            {
                DateToDeleteUtc = DateTime.UtcNow.AddDays(-1) // Example: delete accounts older than 30 days
            };

            // Use MassTransitHelper to send the message to the delete_guest_accounts_queue
            await MassTransitHelper.AddQueueAsync(message, "delete_guest_accounts_queue");

            Console.WriteLine($"[UpdateExchangeRates] Message sent to RabbitMQ to delete guest accounts older than {message.DateToDeleteUtc}");
            Console.WriteLine("[UpdateExchangeRates] Job completed.");
        }
    }

    public class UpdateExchangeRatesMessage
    {
        public DateTime DateToDeleteUtc { get; set; }
    }
}
