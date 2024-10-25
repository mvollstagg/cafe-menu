using Hangfire;
using IAndOthers.Core.IoC;
using Microsoft.Extensions.Logging;
using System;

namespace IAndOthers.Infrastructure.Job
{
    public class HangfireService : IIODependencySingleton
    {
        private readonly ILogger<HangfireService> _logger;

        public HangfireService(ILogger<HangfireService> logger)
        {
            _logger = logger;
        }

        // Fire-and-forget example
        public void EnqueueJob(Action jobAction)
        {
            BackgroundJob.Enqueue(() => ExecuteJob(jobAction));
        }

        // Recurring job example
        public void AddOrUpdateRecurringJob(string jobId, Action jobAction, string cronExpression, string queueName = "default")
        {
            RecurringJob.AddOrUpdate(jobId, () => ExecuteJob(jobAction), cronExpression, queue: queueName);
        }

        // Method to execute jobs with logging
        private void ExecuteJob(Action jobAction)
        {
            try
            {
                _logger.LogInformation($"[HangfireService] Job started at {DateTime.UtcNow}");
                jobAction();
                _logger.LogInformation($"[HangfireService] Job completed at {DateTime.UtcNow}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[HangfireService] Job failed at {DateTime.UtcNow}: {ex.Message}");
                throw;
            }
        }
    }
}
