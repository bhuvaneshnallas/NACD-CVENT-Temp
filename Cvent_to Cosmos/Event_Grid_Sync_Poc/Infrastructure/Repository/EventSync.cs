using Event_Grid_Sync_Poc.Domain.Context;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text.Json; // Add this namespace for AuthenticationHeaderValue
using Event_Grid_Sync_Poc.Domain.Context.Model;

namespace Event_Grid_Sync_Poc.Infrastructure.Repository
{
    public class EventSync
    {
        private readonly IConfiguration _config;
        private readonly CventDBContext _cventDBContext;
        private readonly HttpClient _httpClient;

        public EventSync(CventDBContext cventDBContext, IConfiguration config)
        {
            _cventDBContext = cventDBContext;
            _config = config;
            _httpClient = new HttpClient();
        }

        public async Task FetchDataWithRetryAsync(string moduleName, string fetchAfter)
        {
            // Validated for null in the parent method
            string baseUrl = $"{_config.GetSection("CventApi:CventBaseUrl").Value}/{_config.GetSection("CventApi:Version").Value}/{_config.GetSection($"CventApi:{moduleName}Url").Value}";

            fetchAfter = fetchAfter.Trim('"'); // Remove leading and trailing quotes

            DateTime fetchStartTime = DateTime.TryParse(fetchAfter, out var fetchStart)
                                     ? fetchStart.ToUniversalTime()
                                     : DateTime.MinValue.ToUniversalTime();

            DateTime fetchEndTime = fetchStartTime != DateTime.MinValue.ToUniversalTime()
                ? fetchStartTime.AddMinutes(60).AddMilliseconds(-1).ToUniversalTime()
                : DateTime.MinValue.ToUniversalTime();

            string startTime = fetchStartTime.ToString("yyyy-MM-ddTHH:mm:ss.ffZ");
            string endTime = fetchEndTime.ToString("yyyy-MM-ddTHH:mm:ss.ffZ");

            string url = $"{baseUrl}?after={startTime}&before={endTime}";

            string accessToken = await GetCventToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(responseContent);

            var dataJsonElement = jsonDocument.RootElement.GetProperty("data");
            var data = JsonConvert.DeserializeObject<List<CventEvent>>(dataJsonElement.ToString());
            try
            {
                for (int i = 0; i < data.Count; i++)
                {
                    if (data[i] is CventEvent)
                    {
                        var castedEvent = EventCaster.CreateEvent(data[i] as CventEvent, Guid.NewGuid(), Guid.NewGuid());
                        castedEvent.id = Guid.NewGuid().ToString();
                        castedEvent.IsProcessed = false;
                        await _cventDBContext.Event.AddAsync(castedEvent);
                    }
                }
                _cventDBContext.SaveChanges();
            }
            catch (Exception ex)
            {
                {

                }
            }
        }

        public async Task<string> GetCventToken()
        {
            string token = string.Empty;
            try
            {
                string tokenUrl = $"{_config.GetSection("CventApi:CventBaseUrl").Value}/{_config.GetSection("CventApi:Version").Value}/{_config.GetSection("CventApi:TokenURL").Value}";
                string grant_type = _config.GetSection("CventApi:Grant_Type").Value;
                string client_id = _config.GetSection("CventApi:Client_Id").Value;
                string authorization = _config.GetSection("CventApi:Authorization").Value;
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", grant_type),
                    new KeyValuePair<string, string>("client_id", client_id)
                });
                if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                {
                    _httpClient.DefaultRequestHeaders.Add("Authorization", authorization);
                }
                var response = await _httpClient.PostAsync(tokenUrl, content);
                string responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(responseContent);
                token = jsonResponse["access_token"].ToString();
            }
            catch (Exception ex)
            {
                throw;
            }
            return token;
        }
        public async Task ProcessAndPublishUnprocessedEventsAsync()
        {
            var unprocessedEvents = _cventDBContext.Event
                .Where(e => e.IsProcessed == false)
                .OrderBy(e => e.CreatedDT)
                .Take(10)
                .ToList();

            if (unprocessedEvents.Count == 0)
            {
                Console.WriteLine("No unprocessed events found.");
                return;
            }

            var eventGridService = new Eventgrid(_config);

            //Transform
            var cloudEvents = eventGridService.TransformToCloudEvents(unprocessedEvents);
            await eventGridService.PublishToEventGridAsync(cloudEvents);


            //Mark as processed
            //foreach (var e in unprocessedEvents)
            //{
            //    e.isProcessed = true;
            //    _cventDBContext.Event.Update(e);
            //}

            await _cventDBContext.SaveChangesAsync();
        }

        public async Task UpdateSfId(string sfID,string eventId)
        {
            var ent = _cventDBContext.Event.FirstOrDefault(e => e.id == eventId);
            if (ent != null)
            {
                ent.SFEventId = sfID;
                ent.IsProcessed = true;
            }
            _cventDBContext.SaveChanges();


        }



    }

}