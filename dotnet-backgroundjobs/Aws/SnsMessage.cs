using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace dotnet_backgroundjobs.Aws
{
    public class SnsMessage
    {
        private readonly AmazonSimpleNotificationServiceClient _sns;
        private const string sqsProtocol = "sqs";

        public SnsMessage()
        {
            //TODO: need to add aws key 
            _sns = new AmazonSimpleNotificationServiceClient(
                awsAccessKeyId: "",
                awsSecretAccessKey: "",
                region: Amazon.RegionEndpoint.APSoutheast1);
        }

        public async Task<string> CreateTopic(string topicName)
        {
            var request = new CreateTopicRequest(topicName);
            var response = await _sns.CreateTopicAsync(request);
            return response.TopicArn;
        }

        public async Task<bool> PublishEmailMessage(string topicArn, string message)
        {

            var request = new PublishRequest
            {
                Message = message,
                TopicArn = topicArn
            };

            try
            {
                var response = await _sns.PublishAsync(request);

                Console.WriteLine("Message sent");
                Console.WriteLine(response.MessageId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Caught exception publishing request:");
                Console.WriteLine(ex.Message);
            }
            return true;

        }

        public async Task<bool> SubscribeEmail(string topicArn, string email)
        {
            //subscribe to an SNS topic
            SubscribeRequest subscribeRequest = new SubscribeRequest(topicArn, "email", email);
            SubscribeResponse subscribeResponse = await _sns.SubscribeAsync(subscribeRequest);
            Console.WriteLine("Subscribe RequestId: {0}", subscribeResponse.ResponseMetadata.RequestId);
            Console.WriteLine("Check your email and confirm subscription.");
            return true;
        }

        public async Task<bool> DeleteMessage(string topicArn)
        {
            //delete an SNS topic
            DeleteTopicRequest deleteTopicRequest = new DeleteTopicRequest(topicArn);
            DeleteTopicResponse deleteTopicResponse = await _sns.DeleteTopicAsync(deleteTopicRequest);
            Console.WriteLine("DeleteTopic RequestId: {0}", deleteTopicResponse.ResponseMetadata.RequestId);
            return true;
        }

        public async Task<string> SubscribeQueueToTopic(string queueArn, string topicArn)
        {
            var subScribeRequest = new SubscribeRequest(topicArn, sqsProtocol, queueArn);
            var response = await _sns.SubscribeAsync(subScribeRequest);
            return response.SubscriptionArn;
        }

        public async Task<IEnumerable<string>> ListTopicArns()
        {
            var response = await _sns.ListTopicsAsync(new ListTopicsRequest());
            return response.Topics.Select(x => x.TopicArn).ToList();
        }
    }
}
