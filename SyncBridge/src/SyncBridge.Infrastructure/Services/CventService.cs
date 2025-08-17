using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SyncBridge.Domain.DTOs;
using SyncBridge.Domain.Interfaces;
using SyncBridge.Infrastructure.Data;

namespace SyncBridge.Infrastructure.Services;

public class CventService : ICventService
{
    private readonly IConfiguration _config;

    public CventService(EventsDbContext eventsDbContext, IConfiguration configuration)
    {
        _config = configuration;
    }

    public async Task<List<T>> FetchData<T>(string moduleName, DateTime fetchAfter)
    {
        // Retrieve required configuration values for constructing the API endpoint.
        var baseUrl = _config.GetSection("CventApi:CventBaseUrl")?.Value;
        var version = _config.GetSection("CventApi:Version")?.Value;
        var moduleUrl = _config.GetSection($"CventApi:{moduleName}Url")?.Value;

        // Validate required configuration pieces are present before proceeding.
        if (string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(version) || string.IsNullOrEmpty(moduleUrl))
            throw new InvalidOperationException("Cvent API configuration is missing required values.");

        // Build the initial query URL with a time window filter:
        // 'after' = caller-specified lower bound
        // 'before' = current UTC timestamp (upper bound)
        // Timestamps formatted with fractional seconds and Zulu (UTC) designator.
        var url =
            $"{baseUrl}/{version}/{moduleUrl}?after={fetchAfter:yyyy-MM-ddTHH:mm:ss.ffZ}&before={DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss.ffZ}";

        // Aggregate list for all pages of results.
        List<T> eventList = [];

        // Execute initial request (first page).
        var cventEventResponse = await GetEventsForUrl<T>(url);
        eventList.AddRange(cventEventResponse.data);

        // Follow pagination while a nextToken exists. Means there are more pages to fetch.
        // The API provides the full next page URL in paging._links.next.href.
        while (cventEventResponse.paging.nextToken is not null)
        {
            // Fetch next page using provided link (relies on server-supplied absolute/relative URL).
            cventEventResponse = await GetEventsForUrl<T>(cventEventResponse.paging._links.next.href);

            // Accumulate results from this page.
            eventList.AddRange(cventEventResponse.data);
        }

        return eventList;
    }

    private async Task<PaginatedResult<T>> GetEventsForUrl<T>(string url)
    {
        using var client = new HttpClient();

        // Obtain bearer token for authorization
        var accessToken = await GetToken();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // Issue the GET request to the supplied URL.
        var response = await client.GetAsync(url);

        // Throw if the response indicates failure (ensures exceptions surface upstream).
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();

        // Deserialize JSON into CventEventResponse.
        // If deserialization fails (returns null), return an empty response object to avoid null reference issues.
        // return JsonSerializer.Deserialize<CventEventResponse>(responseContent) ?? new CventEventResponse();
        return JsonConvert.DeserializeObject<PaginatedResult<T>>(responseContent) ?? Activator.CreateInstance<PaginatedResult<T>>();
    }

    public async Task<string> GetToken()
    {
        string tokenUrl =
            $"{_config.GetSection("CventApi:CventBaseUrl").Value}/{_config.GetSection("CventApi:Version").Value}/{_config.GetSection("CventApi:TokenURL").Value}";
        string? grant_type = _config.GetSection("CventApi:Grant_Type")?.Value;
        string? client_id = _config.GetSection("CventApi:Client_Id")?.Value;
        string? authorization = _config.GetSection("CventApi:Authorization")?.Value;

        if (
            string.IsNullOrEmpty(tokenUrl)
            || string.IsNullOrEmpty(grant_type)
            || string.IsNullOrEmpty(client_id)
            || string.IsNullOrEmpty(authorization)
        )
        {
            throw new InvalidOperationException(
                "Cvent API configuration is missing required values for token retrieval."
            );
        }

        var content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("grant_type", grant_type),
                new KeyValuePair<string, string>("client_id", client_id),
            ]
        );

        using var client = new HttpClient();
        if (!client.DefaultRequestHeaders.Contains("Authorization"))
        {
            client.DefaultRequestHeaders.Add("Authorization", authorization);
        }
        var response = await client.PostAsync(tokenUrl, content);
        string responseContent = await response.Content.ReadAsStringAsync();
        var jsonResponse = JObject.Parse(responseContent);
        var token = jsonResponse["access_token"]?.ToString() ?? string.Empty;

        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("Failed to retrieve access token from Cvent API.");
        }

        return token;
    }
}
