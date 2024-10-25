using IAndOthers.Infrastructure.Data;
using IAndOthers.Core.Consumer;
using MassTransit;
using IAndOthers.Core.Data.Services;
using IAndOthers.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using IAndOthers.Application.Job;
using IAndOthers.Core.IoC;

namespace IAndOthers.Consumer.DeleteGuestAccounts
{
    public class DeleteGuestAccountsConsumer : IConsumer<DeleteGuestAccountsMessage>, IIOConsumer
    {
        private readonly IIORepository<AuthGuestUser, ApplicationDbContext> _repository;

        public string QueueName => "delete_guest_accounts_queue";

        public DeleteGuestAccountsConsumer()
        {
            _repository = IODependencyResolver.Resolve<IIORepository<AuthGuestUser, ApplicationDbContext>>();
        }

        public async Task Consume(ConsumeContext<DeleteGuestAccountsMessage> context)
        {
            try
            {
                var dateToDelete = context.Message.DateToDeleteUtc.Date;

                var accountsToDelete = await _repository.Table
                    .Where(a => a.CreationDateUtc.Date < dateToDelete.Date)
                    .AsNoTracking()  // Disable tracking here
                    .ToListAsync();


                if (accountsToDelete.Any())
                {
                    await _repository.DeleteAsync(accountsToDelete);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.UtcNow}] - An error occurred: {ex.Message}");
            }
        }
    }
}
