using AutoMapper;
using Azure;
using Azure.Messaging;
using CventSalesforceSyncApi.Domain.DTO;
using CventSalesforceSyncApi.Domain.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SyncBridge.Domain.Common;
using SyncBridge.Domain.Interfaces;
using SyncBridge.Domain.Models;
using SyncBridge.Domain.Models.CVENT;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


namespace SyncBridge.Application.UseCases
{
    public class SyncEventGridToSalesForce
    {
        private readonly ISalesforceService _salesforceService;
        private readonly ILogger<SyncEventGridToSalesForce> _logger;
        private readonly ISyncLogService _syncLogService;
        private readonly ISettingUrlService _settingUrlService;
        private readonly IAuthenticationTokenServices _authenticationTokenServices;
        private readonly IMapper _mapper;
        private SyncLog? _syncLog;

        public SyncEventGridToSalesForce(ISalesforceService salesforceService, ILogger<SyncEventGridToSalesForce> logger,ISyncLogService syncLogService, ISettingUrlService settingUrlService, IAuthenticationTokenServices authenticationTokenServices, IMapper mapper)
        {
            _salesforceService = salesforceService;
            _logger = logger;
            _syncLogService = syncLogService;
            _settingUrlService = settingUrlService;
            _authenticationTokenServices = authenticationTokenServices;
            _mapper = mapper;
        }

        public async Task EventGridToSalesForce(CloudEvent cloudEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _syncLog = System.Text.Json.JsonSerializer.Deserialize<SyncLog>(
                   cloudEvent.Data,
                   new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });


                if (_syncLog == null || string.IsNullOrEmpty(_syncLog.id))
                {
                    _logger.LogWarning("Failed to deserialize _syncLog or missing id from cloud event data");
                    return;
                }

                SyncLogUpdate syncLogUpdate = new SyncLogUpdate
                {
                    id = _syncLog.id,
                    HandlerName = SyncLogConstants.Handler.CVENTtoSFunction,
                    Status = _syncLog.Action == SyncLogConstants.Action.Retry ? SyncLogConstants.Status.Retry_Inititated : SyncLogConstants.Status.InProgress,
                };
                var synclog = await _syncLogService.UpdateSyncLog(syncLogUpdate, _logger);

                var (sourceDataUrl,sourceUpdateUrl, destinationSfUrl, destination_Endpoint_Prefix) = await _settingUrlService.GetSalesforceDestionSourceURL(synclog.Module);

                if (!string.IsNullOrEmpty(_syncLog.RecordId))
                {
                    sourceDataUrl = $"{sourceDataUrl}{_syncLog.RecordId}";
                    //sourceUrl = $"https://app-cvent-middleware-eds-eastus-stg.azurewebsites.net/api/v1/Event/{syncLog.RecordId}";
                }

                if (destination_Endpoint_Prefix.Equals(Constants.SFKEYPREFIX))
                {
                    var sourceData = "";
                    sourceData = GetExternalRecords(sourceDataUrl, _syncLog.Module).Result;

                    HttpResponseMessage response = null;

                    if (sourceData != null)
                    {
                        var mappingData = ReadDto(_syncLog.Module, sourceData);
                        response = await _salesforceService.SalesforceApiCall(mappingData, destinationSfUrl);
                    }

                    if (response != null)
                    {

                        var responseBody = await response.Content.ReadAsStringAsync();

                        SyncLogUpdate sfResultSyncLog = new SyncLogUpdate()
                        {
                            id = synclog.id,
                            HandlerName = "Handler-3",
                        };
                        if (!response.IsSuccessStatusCode)
                        {
                            sfResultSyncLog.Status = SyncLogConstants.Status.Failed;
                            sfResultSyncLog.ErrorCode = SyncLogConstants.ErrorCode.SF_Failed;
                            sfResultSyncLog.Error = responseBody.ToString();
                            await _syncLogService.UpdateSyncLog(sfResultSyncLog, _logger);
                            // return $"Error posting to Salesforce. Status: {response.StatusCode}, Response: {responseBody}";
                        }
                        else
                        {
                            sfResultSyncLog.Status = SyncLogConstants.Status.Completed;

                            await _syncLogService.UpdateSyncLog(sfResultSyncLog, _logger);

                            var updateParams = JsonConvert.DeserializeObject<List<SfResponse>>(responseBody);

                            await UpdateCosmosAPICall(updateParams, sourceUpdateUrl);

                            //  return $"Successfully posted {json} to Salesforce.{responseBody}";
                        }
                    }
                    else
                    {
                        _logger.LogInformation(@$"No Records found for {sourceDataUrl}");
                    }

                }
            }
            catch (Exception ex)
            {
                // TODO: log exception
                throw;
            }
        }

        public async Task<string> GetExternalRecords(string sourceUrl, string module)
        {
            //JObject jsonData; // Single Record
            object jsonData;

            try
            {
                var client = new HttpClient();
                var accessToken = _authenticationTokenServices.GetCosmosAuthToken().Result;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


                var responses = await client.GetAsync(new Uri(sourceUrl));
                var apiResponse = await responses.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(apiResponse);

                //  jsonData = (JArray)jsonObject.GetValue("data");
                jsonData = jsonObject.GetValue("data");

            }
            catch (Exception ex)
            {
                _logger.LogInformation(@$"No Records found for {sourceUrl}");
                throw ex;
            }
            return CheckDependencies(jsonData, module) ? JsonConvert.SerializeObject(jsonData) : null;
        }

        private bool CheckDependencies(object jsonData, string module)
        {
            var requiredFields = module switch
            {
                "TicketType" => new[] { "sfEventId" },
                "Attendee" => new[] { "sfContactId", "sfEventId", "sfTicketTypeId" },
                "SalesOrder" => new[] { "sfContactId", "sfEventId" },
                "Receipt" => new[] { "sfContactId", "sfOrderId" },
                _ => Array.Empty<string>()
            };

            if (jsonData is not JObject jObject)
                return false; // only JObject supported here

            if (requiredFields.Length == 0)
                return true;

            // Track missing fields
            List<string> missingErrors = new();

            foreach (var field in requiredFields)
            {
                if (!jObject.TryGetValue(field, out var value) || string.IsNullOrEmpty(value?.ToString()))
                {
                    string errorCode = field switch
                    {
                        "sfEventId" => SyncLogConstants.Error.SF_EventId_Missing,
                        "sfContactId" => SyncLogConstants.Error.SF_ContactId_Missing,
                        "sfTicketTypeId" => SyncLogConstants.Error.SF_TicketTypeId_Missing,
                        "sfOrderId" => SyncLogConstants.Error.SF_OrderId_Missing,
                        _ => $"{SyncLogConstants.Error.SF_DependencyError}:{field}_Missing"
                    };

                    missingErrors.Add(errorCode);
                }
            }

            bool isValidRecord = missingErrors.Count == 0;

            if (!isValidRecord)
            {
                SyncLogUpdate syncLogUpdate = new SyncLogUpdate()
                {
                    id = _syncLog.id,
                    Status = SyncLogConstants.Status.Failed,
                    ErrorCode = SyncLogConstants.ErrorCode.SF_Failed,
                    // Join multiple errors with commas
                    Error = string.Join(", ", missingErrors)
                };

                _syncLogService.UpdateSyncLog(syncLogUpdate, _logger);
            }

            return isValidRecord;
        }

        private dynamic ReadDto(string module, string json)
        {
            dynamic dto = null;

            try
            {
                switch (module)
                {
                    case SFConstants.EVENT:
                        var eventData = JsonConvert.DeserializeObject<EventFromSyncAPI>(json);
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

        public async Task<string> UpdateCosmosAPICall<T>(T requestBody, string apiUrl)
        {
            HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
            var items = "";
            string accessToken = string.Empty;
            try
            {
                var json = JsonConvert.SerializeObject(requestBody);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                var client = new HttpClient();
                accessToken = _authenticationTokenServices.GetCosmosAuthToken().Result;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var responses = await client.PutAsync(new Uri(apiUrl), stringContent);
                items = await responses.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(@$"Something went wrong while passing the Cvent payload. {ex}");
                throw ex;
            }
            return items;
        }


    }
}
