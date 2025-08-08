using Azure;
using Azure.Messaging;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using WebApplication1.Models;

public class CventEventRepository
{
    private readonly ILogger<CventEventRepository> _logger;
    private readonly IConfiguration configuration;
    private readonly string apiUrl = "https://nacdonline--fullcopy3.sandbox.my.salesforce.com/services/apexrest/EventApi__Event__c";

    public CventEventRepository(ILogger<CventEventRepository> logger, IConfiguration configuration)
    {
        _logger = logger;
        this.configuration = configuration;
    }

    public async Task HandleEventAsync(CloudEvent cloudEvent, int count)
    {
        if (cloudEvent.Type == "Cvent.Event.Created")
        {
            try
            {
                var cventEvent = System.Text.Json.JsonSerializer.Deserialize<CventEvent>(
                    cloudEvent.Data.ToString()!,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                _logger.LogInformation("Deserialized Event: {Json}",
    System.Text.Json.JsonSerializer.Serialize(cventEvent, new JsonSerializerOptions { WriteIndented = true }));


                await SyncDataToSalesforce(cventEvent);
                var accessToken = GetToken(configuration).Result;


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize Cvent event.");
            }
        }

        await Task.CompletedTask;
    }

    public async Task SyncDataToSalesforce(CventEvent sfInput)
    {
        try
        {
            _logger.LogInformation($"Starting sync for event: {sfInput?.Title}");

            // Map to clean DTO
            var eventDto = MapToEventDto(sfInput);
            var json = JsonConvert.SerializeObject(new List<EventDto>() { eventDto });

            _logger.LogInformation("Data To API: {Json}",
    System.Text.Json.JsonSerializer.Serialize(eventDto, new JsonSerializerOptions { WriteIndented = true }));


            using var client = new HttpClient();
            var accessToken = await GetToken(configuration);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(new Uri(apiUrl), content);
            var responseBody = await response.Content.ReadAsStringAsync();

            var items = System.Text.Json.JsonSerializer.Deserialize<List<SalesforceResponseItem>>(responseBody);

            await updateSFId(items);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error posting to Salesforce. Status: {response.StatusCode}, Response: {responseBody}");
            }
            else
            {
                _logger.LogInformation($"Successfully posted event: {eventDto.Title} to Salesforce.{responseBody}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while syncing data to Salesforce.");
            throw;
        }
    }

    public async Task updateSFId(List<SalesforceResponseItem> items)
    {
        var baseUrl = "http://localhost:5296/api/EventSync/UpdateSFEventId";

        using var client = new HttpClient();

        var requestUrl = $"{baseUrl}?sdId={items[0].SfId}&eventId={items[0].Id}";

        var request = new HttpRequestMessage(HttpMethod.Patch, requestUrl);

        request.Content = JsonContent.Create(new { });

        try
        {
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("✅ SFEventId updated successfully.");
            }
            else
            {
                Console.WriteLine($"❌ Failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❗ Exception: {ex.Message}");
        }
    }

    public async Task<string> GetToken(IConfiguration configuration)
    {
        string accessToken = string.Empty;
        try
        {
            var url = configuration.GetSection("SalesforceApiConfig:TokenApi").Value;
            using (var client = new HttpClient())
            {
                var reqContent = new Dictionary<string, string>();
                reqContent.Add("username", configuration.GetSection("SalesforceApiConfig:username").Value);
                reqContent.Add("password", configuration.GetSection("SalesforceApiConfig:password").Value);
                reqContent.Add("grant_type", configuration.GetSection("SalesforceApiConfig:grant_type").Value);
                reqContent.Add("client_id", configuration.GetSection("SalesforceApiConfig:client_id").Value);
                reqContent.Add("client_secret", configuration.GetSection("SalesforceApiConfig:client_secret").Value);
                using (var content = new FormUrlEncodedContent(reqContent))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                    content.Headers.ContentType.CharSet = "UTF-8";
                    client.DefaultRequestHeaders.ExpectContinue = false;
                    HttpResponseMessage response = await client.PostAsync(new Uri(url), content);
                    var tokenResponse = response.Content.ReadAsStringAsync().Result;
                    var jsonResponse = JsonConvert.DeserializeObject<SalesForceTokenResponse>(tokenResponse);
                    accessToken = jsonResponse.Access_token;
                }

            }
        }
        catch (Exception ex)
        {
            throw;
        }
        return accessToken;
    }

    public static EventDto MapToEventDto(CventEvent sfInput)
    {
        return new EventDto
        {
            id = sfInput.id ?? Guid.NewGuid().ToString(),
            ExternalEventId = sfInput.ExternalEventId ?? "",
            Title = sfInput.Title ?? "Untitled Event",
            Status = sfInput.Status ?? "Draft",
            StartDate = sfInput.StartDate ?? DateTime.UtcNow,
            EndDate = sfInput.EndDate ?? DateTime.UtcNow.AddDays(1),
            Description = sfInput.Description ?? "abcd",
            Code = sfInput.Code ?? "N/A",
            Category = sfInput.Category ?? "General",
            Capacity = sfInput.Capacity.ToString() ?? "1",
            Conference = sfInput.Conference ?? "Not Specified",
            EventFormat = sfInput.EventFormat ?? "Conference",
            LaunchDate =  DateOnly.FromDateTime(DateTime.UtcNow),
            TimeZone = sfInput.TimeZone ?? "UTC",
            SFEventId = sfInput.SFEventId ?? "",
            BatchId = sfInput.BatchId ?? ""
        };

    }

}
public class SalesforceResponseItem
{
    public string BatchId { get; set; }
    public string SfId { get; set; }
    public string Id { get; set; }
}


public class SalesForceTokenResponse
{
    public string Access_token { get; set; }
    public string Instance_url { get; set; }
    public string Id { get; set; }
    public string Token_type { get; set; }
    public string Issued_at { get; set; }
    public string Signature { get; set; }
}