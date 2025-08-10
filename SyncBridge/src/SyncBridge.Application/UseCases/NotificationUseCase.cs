using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SyncBridge.Domain.Models;

namespace SyncBridge.Application.UseCases
{
    public class NotificationUseCase
    {
        public async Task SendNotification(
            Notification Notification,
            HttpClient _httpClient,
            IConfiguration configuration,
            ILogger log
        )
        {
            var adaptiveCardJson =
                $@"{{
            ""type"": ""message"",
            ""attachments"": [
            {{
                  ""contentType"": ""application/vnd.microsoft.card.adaptive"",
                  ""content"":
                  {{ 
                       ""type"": ""AdaptiveCard"",
                       ""$schema"": ""http://adaptivecards.io/schemas/adaptive-card.json"",
                       ""version"": ""1.4"",
                       ""body"": {BuildAdaptiveCardBody(Notification)}
                        }}
                     }}
                  ]
            }}";

            var webhookUrl = configuration.GetSection("TeamsNotificationWebhookUrl").Value;
            var content = new StringContent(adaptiveCardJson, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(webhookUrl, content);

            if (response.IsSuccessStatusCode)
            {
                log.LogInformation("Message sent to Teams channel successfully.");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                log.LogError(
                    "Failed to send message to Teams. Status Code: {StatusCode}, Error: {Error}",
                    response.StatusCode,
                    error
                );
            }
        }

        private string BuildAdaptiveCardBody(Notification notification)
        {
            return $@"[
            {{
                ""type"": ""TextBlock"",
                ""text"": ""Alert: Data Sync Failures {notification.Source}-{notification.Destination}"",
                ""weight"": ""Bolder"",
                ""size"": ""Medium"",
                ""wrap"": true
            }},
            {{
                ""type"": ""FactSet"",
                ""facts"": [
                    {{ ""title"": ""Application:"", ""value"": ""{notification.Application}"" }},
                    {{ ""title"": ""Source:"", ""value"": ""{notification.Source}"" }},
                    {{ ""title"": ""Destination:"", ""value"": ""{notification.Destination}"" }},
                    {{ ""title"": ""Record Name:"", ""value"": ""{notification.RecordName}"" }},
                    {{ ""title"": ""Record Id:"", ""value"": ""{notification.RecordId}"" }},
                    {{ ""title"": ""Start Time:"", ""value"": ""{notification.StartTime}"" }},
                    {{ ""title"": ""End Time:"", ""value"": ""{notification.Endtime}"" }},
                    {{ ""title"": ""Status:"", ""value"": ""{notification.Status}"" }},
                    {{ ""title"": ""Error:"", ""value"": ""{notification.Error}"" }},

                ]
            }}
            ]";
        }
    }
}
