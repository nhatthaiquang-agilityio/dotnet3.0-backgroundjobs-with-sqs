using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Annotations;
using Hangfire.Server;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace dotnet_backgroundjobs.Tasks
{
    public class RecurringJobsService : BackgroundService
    {
        private readonly IRecurringJobManager _recurringJobs;
        private readonly ILogger<RecurringJobScheduler> _logger;

        public RecurringJobsService(
            [NotNull] IRecurringJobManager recurringJobs,
            [NotNull] ILogger<RecurringJobScheduler> logger)
        {
            _recurringJobs = recurringJobs ?? throw new ArgumentNullException(nameof(recurringJobs));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Run Jobs");

                BackgroundJob.Enqueue<TasksService>(x => x.Cancelable(10, JobCancellationToken.Null));

                //_recurringJobs.AddOrUpdate("seconds", () => Console.WriteLine("Hello, seconds!"), "*/15 * * * * *");
                _recurringJobs.AddOrUpdate("minutely", () => Console.WriteLine("Hello, world!"), Cron.Minutely);
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred while creating recurring jobs.", e);
            }

            await Task.Delay(1000, stoppingToken);
        }

    }
}