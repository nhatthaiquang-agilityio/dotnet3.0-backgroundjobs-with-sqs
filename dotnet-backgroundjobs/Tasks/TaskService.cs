using System;
using System.Threading.Tasks;
using Hangfire;

namespace dotnet_backgroundjobs.Tasks
{
    public class TasksService
    {
        public async Task Cancelable(int iterationCount, IJobCancellationToken token)
        {
            try
            {
                for (var i = 1; i <= iterationCount; i++)
                {
                    await Task.Delay(1000);
                    Console.WriteLine("Performing step {0} of {1}...", i, iterationCount);

                    token.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Cancellation requested, exiting...");
                throw;
            }
        }


        public async Task RunUploadImage()
        {
            try
            {
                Console.WriteLine("Run Job Upload Image");
                Console.WriteLine("TODO: Define Logic Uploading Image");
                // TODO: logic upload image
                await Task.Delay(100);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Cancellation requested, exiting...");
                throw;
            }
        }

        public async Task RunProcessMessages(string message)
        {
            try
            {
                Console.WriteLine("Run Job Processing Message");
                Console.WriteLine(message);
                await Task.Delay(100);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Cancellation requested, exiting...");
                throw;
            }
        }
    }
}