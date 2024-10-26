using IAndOthers.Core.Jobs;
using IAndOthers.Core.MassTransit;
using MassTransit;

namespace IAndOthers.Application.Job
{
    public class DeleteGuestAccountsJob : IORecurringJob
    {
        public override string Code => "DeleteGuestAccounts";
        public override string Cron => "0 0 * * *";

        public override async Task Execute()
        {
            Console.WriteLine("[DeleteGuestAccounts] Executing the job to remove guest accounts...");

            var message = new DeleteGuestAccountsMessage
            {
                DateToDeleteUtc = DateTime.UtcNow.AddDays(-1)
            };

            await MassTransitHelper.AddQueueAsync(message, "delete_guest_accounts_queue");

            Console.WriteLine("[DeleteGuestAccounts] Job completed.");
        }
    }

    public class DeleteGuestAccountsMessage
    {
        public DateTime DateToDeleteUtc { get; set; }
    }
}
