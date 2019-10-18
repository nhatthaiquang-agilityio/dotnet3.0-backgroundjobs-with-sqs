using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace dotnet_backgroundjobs.Services
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            const string apiKey = "key-ef7a2525b9a4141408b40cd4d4e438e0";
            const string sandBox = "sandbox5c2ed57ac7b94f0ea5d372f3194b026c.mailgun.org";
            byte[] apiKeyAuth = Encoding.ASCII.GetBytes($"api:{apiKey}");
            var httpClient = new HttpClient { BaseAddress = new Uri("https://api.mailgun.net/v3/") };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(apiKeyAuth));

            var form = new Dictionary<string, string>
            {
                ["from"] = "postmaster@sandbox5c2ed57ac7b94f0ea5d372f3194b026c.mailgun.org",
                ["to"] = message.Destination,
                ["subject"] = message.Subject,
                ["text"] = message.Body
            };

            HttpResponseMessage response =
                httpClient.PostAsync(sandBox + "/messages", new FormUrlEncodedContent(form)).Result;
            return Task.FromResult((int)response.StatusCode);
        }
    }
}
