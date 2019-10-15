using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SQS.Model;
using dotnet_backgroundjobs.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace dotnet_backgroundjobs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly SqsMessage sqsMessage;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            sqsMessage = new SqsMessage();
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        // API: call upload image job
        [HttpGet]
        [Route("/upload_image")]
        public void RunJobUploadImage()
        {
            BackgroundJob.Enqueue<TasksService>(x => x.RunUploadImage());
        }

        [HttpGet]
        [Route("/send_message/{content:minlength(1)}")]
        public async Task<IActionResult> SendSQSMessage(string content)
        {
            await sqsMessage.SendSQSMessage(content);
            return Ok("Done");
        }

        [HttpGet]
        [Route("/receive_message")]
        public async Task<List<Message>> ReceiveSQSMessage()
        {
            List<Message> messages = await sqsMessage.ReceiveSQSMessage();
            return messages;
        }

        [HttpPost]
        [Route("/delete_message")]
        public async Task<IActionResult> DeleteSQSMessage([FromBody] Message body)
        {
            Console.WriteLine("Post delete Message");
            await sqsMessage.DeleteSQSMessage(body.ReceiptHandle);
            return Ok(body.ReceiptHandle);
        }
    }
}
