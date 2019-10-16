using System;
using System.Threading;
using System.Threading.Tasks;
using dotnet_backgroundjobs.Tasks;
using dotnet_backgroundjobs.Aws;
using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace dotnet_backgroundjobs
{
    public class QueueReaderService : BackgroundService
    {
        private readonly ILogger<QueueReaderService> _logger;
        private readonly SqsMessage _sqsMessage;

        public QueueReaderService(ILogger<QueueReaderService> logger, SqsMessage sqsMessage)
        {
            _logger = logger;
            _sqsMessage = sqsMessage;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("QueueReaderService is starting.");
            
            try
            {
                while (!stoppingToken.IsCancellationRequested) // keep reading while the app is running.
                {
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
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            
        }

    }
}

