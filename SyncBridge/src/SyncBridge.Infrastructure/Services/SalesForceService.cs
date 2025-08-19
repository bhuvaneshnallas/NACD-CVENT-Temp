using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SyncBridge.Domain.Interfaces;
using SyncBridge.Domain.Models.CVENT;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SyncBridge.Domain.DTOs;
using CventSalesforceSyncApi.Domain.DTO;
using System.Reflection.Metadata;
using CventSalesforceSyncApi.Domain.Utilities;
using AutoMapper;
using AutoMapper.Execution;
using SyncBridge.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace SyncBridge.Infrastructure.Services
{
    public class SalesForceService : ISalesforceService
    {

        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly string _apiUrl = "https://nacdonline--fullcopy3.sandbox.my.salesforce.com/services/apexrest/EventApi__Event__c";
        private readonly ISyncLogService _syncLogService;
        private readonly ILogger<SalesForceService> _logger;

        public SalesForceService(IMapper mapper, IConfiguration configuration, ISyncLogService syncLogService, ILogger<SalesForceService> logger)
        {
            _configuration = configuration;
            _mapper = mapper;
            _syncLogService = syncLogService;
            _logger = logger;
        }


        public async Task<string> SalesForcePost<T>(T entity)
        {

            //Regiester In Sync Log DB
            QueueModel queueModel = _mapper.Map<QueueModel>(entity);
            await _syncLogService.CreateSyncLog(queueModel, _logger);


            //TODO: SalesForce POST URL Configuration Needed
            string json;

            switch (entity)
            {
                case EventEntity e:
                    var eventDto = ReadDto(SFConstants.EVENT, entity);
                    json = JsonConvert.SerializeObject(new List<EventDto> { eventDto });
                    break;

                case Attendee a:
                    var attendeeDto = ReadDto(SFConstants.EVENT, entity);
                    json = JsonConvert.SerializeObject(new List<AttendeeDto> { attendeeDto });
                    break;

                case TicketType t:
                    var ticketDto = ReadDto(SFConstants.EVENT, entity);
                    json = JsonConvert.SerializeObject(new List<TicketTypeDto> { ticketDto });
                    break;

                case SyncBridge.Domain.Models.CVENT.Contact c:
                    var contactDto = ReadDto(SFConstants.EVENT, entity);
                    json = JsonConvert.SerializeObject(new List<SyncBridge.Domain.Models.CVENT.Contact> { contactDto });
                    break;

                case SalesOrder s:
                    var salesOrderDto = ReadDto(SFConstants.EVENT, entity);
                    json = JsonConvert.SerializeObject(new List<SalesOrderDto> { salesOrderDto });
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported entity type: {typeof(T).Name}");
            }


            // Now send json to Salesforce
            using (var client = new HttpClient())
            {
                var accessToken = await GetToken(_configuration);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(new Uri(_apiUrl), content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                     await _syncLogService.UpdateSyncLog(queueModel.id, _logger, "Handler-3", SyncLogConstants.Status.Failed, SyncLogConstants.ErrorCode.SF_Failed, responseBody.ToString());

                    return $"Error posting to Salesforce. Status: {response.StatusCode}, Response: {responseBody}";
                }
                else
                {
                    await _syncLogService.UpdateSyncLog(queueModel.id, _logger, "Handler-3", SyncLogConstants.Status.Completed,"", responseBody.ToString());

                    var items = System.Text.Json.JsonSerializer.Deserialize<List<SalesforceResponseItem>>(responseBody);

                    await updateSFId(items);

                    return $"Successfully posted {json} to Salesforce.{responseBody}";
                }
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


        /// <summary>
        /// Assign Dynamic Dto
        /// </summary>
        /// <param name="module"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private dynamic ReadDto(string module, object entity)
        {
            dynamic dto = null;

            var json = JsonConvert.SerializeObject(entity);
            try
            {
                switch (module)
                {
                    case SFConstants.EVENT:
                        var eventData = JsonConvert.DeserializeObject<EventEntity>(json);

                        dto = _mapper.Map<EventDto>(eventData);
                        return dto;


                    case SFConstants.SALESORDER:
                        var salesOrderData = JsonConvert.DeserializeObject<SalesOrderDto>(json);
                        return salesOrderData;

                    case SFConstants.ATTENDEE:

                        var attendeeData = JsonConvert.DeserializeObject<Attendee>(json);
                        dto = _mapper.Map<AttendeeDto>(attendeeData);
                        return dto;

                    case SFConstants.RECEIPT:
                        var receiptData = JsonConvert.DeserializeObject<ReceiptDto>(json);
                        return receiptData;

                    case SFConstants.TICKETTYPE:

                        var ticketTypeData = JsonConvert.DeserializeObject<TicketType>(json);
                        dto = _mapper.Map<TicketTypeDto>(ticketTypeData);
                        return dto;

                    default:
                        throw new InvalidOperationException("Unknown module type.");

                }
            }
            catch (Exception ex)
            {
                // _log.LogInformation(@$"Mapping Exception : {ex}");
                throw ex;
            }
        }

        //TODO: forlocal testing remove it
        public async Task updateSFId(List<SalesforceResponseItem> items)
        {
            var baseUrl = "http://localhost:5296/api/EventSync/UpdateSFEventId";

            using var client = new HttpClient();

            var requestUrl = $"{baseUrl}?sdId={items[0].SfId}&eventId={items[0].Id}";

            var request = new HttpRequestMessage(HttpMethod.Patch, requestUrl);

            request.Content = JsonContent.Create(new { });

            var response = await client.SendAsync(request);

        }
    }

            
}
