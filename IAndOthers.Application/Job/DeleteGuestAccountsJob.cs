using IAndOthers.Core.Jobs;
using IAndOthers.Core.MassTransit;
using MassTransit;

namespace IAndOthers.Application.Job
{
    public class DeleteGuestAccountsJob : IORecurringJob
    {
        public override string Code => "DeleteGuestAccounts";
        public override string Cron => "0 0 * * *"; // Runs every day at midnight

        public override async Task Execute()
        {
            // Your RabbitMQ message logic
            Console.WriteLine("[DeleteGuestAccounts] Executing the job to remove guest accounts...");

            // Prepare the message to send to RabbitMQ
            var message = new DeleteGuestAccountsMessage
            {
                DateToDeleteUtc = DateTime.UtcNow.AddDays(-1) // Example: delete accounts older than 30 days
            };

            // Use MassTransitHelper to send the message to the delete_guest_accounts_queue
            await MassTransitHelper.AddQueueAsync(message, "delete_guest_accounts_queue");

            Console.WriteLine($"[DeleteGuestAccounts] Message sent to RabbitMQ to delete guest accounts older than {message.DateToDeleteUtc}");
            Console.WriteLine("[DeleteGuestAccounts] Job completed.");
        }
    }

    public class DeleteGuestAccountsMessage
    {
        public DateTime DateToDeleteUtc { get; set; }
    }
}
