using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
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
        private readonly SqsMessage _sqsMessage;

        public RecurringJobsService(
            [NotNull] IRecurringJobManager recurringJobs,
            [NotNull] ILogger<RecurringJobScheduler> logger,
            SqsMessage sqsMessage)
        {
            _recurringJobs = recurringJobs ?? throw new ArgumentNullException(nameof(recurringJobs));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sqsMessage = sqsMessage;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Run Jobs");

                BackgroundJob.Enqueue<TasksService>(x => x.Cancelable(10, JobCancellationToken.Null));

                //_recurringJobs.AddOrUpdate("seconds", () => Console.WriteLine("Hello, seconds!"), "*/15 * * * * *");
                _recurringJobs.AddOrUpdate("minutely", () => Console.WriteLine("Hello, world!"), Cron.Minutely);

                while (!stoppingToken.IsCancellationRequested) // keep reading while the app is running.
                {

                    Console.WriteLine("Reading SQS");
                    try
                    {
                        var messages = await _sqsMessage.ReceiveSQSMessage();
                        if (messages != null)
                        {
                            foreach (var message in messages)
                            {
                                BackgroundJob.Enqueue<TasksService>(
                                    x => x.RunProcessMessages(message.Body));
                                await _sqsMessage.DeleteSQSMessage(message.ReceiptHandle);
                            }
                        }
                    }
                    catch (AmazonSQSException ex)
                    {
                        _logger.LogError(ex.Message);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred while creating recurring jobs.", e);
            }

            await Task.Delay(1000, stoppingToken);
        }

    }
}