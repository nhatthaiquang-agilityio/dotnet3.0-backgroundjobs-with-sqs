using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using dotnet_backgroundjobs.Aws;
using dotnet_backgroundjobs.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Amazon.SQS.Model;

namespace dotnet_backgroundjobs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AwsController : ControllerBase
    {
        private readonly SnsMessage _snsMessage;
        private readonly SqsMessage _sqsMessage;

        private const string TopicArn = "arn:aws:sns:ap-southeast-1:783560535431:MyTopic";

        public AwsController(SnsMessage snsMessage, SqsMessage sqsMessage)
        {
            _snsMessage = snsMessage;
            _sqsMessage = sqsMessage;
        }

        /* Amazon Simple Queue Service */
        // API: call upload image job
        [HttpGet]
        [Route("sqs/upload_image")]
        public void RunJobUploadImage()
        {
            BackgroundJob.Enqueue<TasksService>(x => x.RunUploadImage());
        }

        [HttpGet]
        [Route("sqs/send_message/{content:minlength(1)}")]
        public async Task<IActionResult> SendSQSMessage(string content)
        {
            await _sqsMessage.SendSQSMessage(content);
            return Ok("Done");
        }

        [HttpGet]
        [Route("sqs/receive_message")]
        public async Task<List<Message>> ReceiveSQSMessage()
        {
            List<Message> messages = await _sqsMessage.ReceiveSQSMessage();
            return messages;
        }

        [HttpPost]
        [Route("sqs/delete_message")]
        public async Task<IActionResult> DeleteSQSMessage([FromBody] Message body)
        {
            Console.WriteLine("Post delete Message");
            await _sqsMessage.DeleteSQSMessage(body.ReceiptHandle);
            return Ok(body.ReceiptHandle);
        }

        /* Simple Notification Service */
        [HttpPost]
        [Route("sns/publish_message")]
        public async Task<IActionResult> PublishMessage()
        {
            Console.WriteLine("Publish Message");
            var message = "Testing Simple Notification Service via Email";
            await _snsMessage.PublishEmailMessage(TopicArn, message);
            return Ok("Done");
        }

        [HttpGet]
        [Route("sns/subscribe_message/{email:minlength(1)}")]
        public async Task<IActionResult> SubscribeMessage(string email)
        {
            Console.WriteLine("Subscribe Message");
            await _snsMessage.SubscribeEmail(TopicArn, email);
            return Ok("Done");
        }
    }
}
