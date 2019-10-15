using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace dotnet_backgroundjobs
{
    public class SqsMessage
    {
        private readonly IAmazonSQS sqs;
        private readonly string myQueueUrl = "https://sqs.ap-southeast-1.amazonaws.com/783560535431/MyQueue1";

        public SqsMessage()
        {
            sqs = new AmazonSQSClient(RegionEndpoint.APSoutheast1);
        }

        public async Task SendSQSMessage(string content)
        {
            try
            {
                Console.WriteLine("===========================================");
                Console.WriteLine("Getting Started with Amazon SQS");
                Console.WriteLine("===========================================\n");

                //Creating a queue
                //Console.WriteLine("Create a queue called MyQueue.\n");
                //CreateQueueRequest sqsRequest = new CreateQueueRequest();
                //sqsRequest.QueueName = "MyQueue";
                //CreateQueueResponse createQueueResponse = await sqs.CreateQueueAsync(sqsRequest);
                //String myQueueUrl = "https://sqs.ap-southeast-1.amazonaws.com/783560535431/MyQueue1";
                //myQueueUrl = createQueueResponse.QueueUrl;

                //Confirming the queue exists
                ListQueuesRequest listQueuesRequest = new ListQueuesRequest();
                ListQueuesResponse listQueuesResponse = await sqs.ListQueuesAsync(listQueuesRequest);

                Console.WriteLine("Printing list of Amazon SQS queues.\n");
                foreach (String queueUrl in listQueuesResponse.QueueUrls)
                {
                    Console.WriteLine("  QueueUrl: {0}", queueUrl);
                }
                Console.WriteLine();

                //Sending a message
                Console.WriteLine("Sending a message to MyQueue.\n");
                SendMessageRequest sendMessageRequest = new SendMessageRequest();
                sendMessageRequest.QueueUrl = myQueueUrl; //URL from initial queue creation
                sendMessageRequest.MessageBody = content;
                await sqs.SendMessageAsync(sendMessageRequest);
            }
            catch (AmazonSQSException ex)
            {
                Console.WriteLine("Caught Exception: " + ex.Message);
                Console.WriteLine("Response Status Code: " + ex.StatusCode);
                Console.WriteLine("Error Code: " + ex.ErrorCode);
                Console.WriteLine("Error Type: " + ex.ErrorType);
                Console.WriteLine("Request ID: " + ex.RequestId);
            }
            return;
        }

        public async Task DeleteSQSMessage(string messageRecieptHandle)
        {
            //Deleting a message
            Console.WriteLine("Deleting the message.\n");
            DeleteMessageRequest deleteRequest = new DeleteMessageRequest();
            deleteRequest.QueueUrl = myQueueUrl;
            deleteRequest.ReceiptHandle = messageRecieptHandle;
            await sqs.DeleteMessageAsync(deleteRequest);
            return;
        }

        public async Task<List<Message>> ReceiveSQSMessage()
        {
            //Receiving a message
            ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest();
            receiveMessageRequest.QueueUrl = myQueueUrl;

            try
            {
                ReceiveMessageResponse receiveMessageResponse =
                    await sqs.ReceiveMessageAsync(receiveMessageRequest);

                if (receiveMessageResponse.Messages.Any())
                { 
                    //Console.WriteLine("Printing received message.\n");
                    //foreach (Message message in receiveMessageResponse.Messages)
                    //{
                    //    Console.WriteLine("  Message");
                    //    Console.WriteLine("    MessageId: {0}", message.MessageId);
                    //    Console.WriteLine("    ReceiptHandle: {0}", message.ReceiptHandle);
                    //    Console.WriteLine("    MD5OfBody: {0}", message.MD5OfBody);
                    //    Console.WriteLine("    Body: {0}", message.Body);

                    //    foreach (KeyValuePair<string, string> entry in message.Attributes)
                    //    {
                    //        Console.WriteLine("  Attribute");
                    //        Console.WriteLine("    Name: {0}", entry.Key);
                    //        Console.WriteLine("    Value: {0}", entry.Value);
                    //    }
                    //}
                    //String messageRecieptHandle = receiveMessageResponse.Messages[0].ReceiptHandle;

                    //Console.WriteLine("Message handle:");
                    //Console.WriteLine(messageRecieptHandle);
                    return receiveMessageResponse.Messages;
                }
                return null;
            }
            catch (AmazonSQSException)
            {
                return null;
            }
        }
    }
}
