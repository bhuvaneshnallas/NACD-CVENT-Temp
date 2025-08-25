using AutoMapper;
using AutoMapper.Execution;
using Azure.Messaging;
using CventSalesforceSyncApi.Domain.DTO;
using CventSalesforceSyncApi.Domain.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SyncBridge.Domain.Common;
using SyncBridge.Domain.DTOs;
using SyncBridge.Domain.Interfaces;
using SyncBridge.Domain.Models;
using SyncBridge.Domain.Models.CVENT;
using SyncBridge.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace SyncBridge.Infrastructure.Services
{
    public class SalesForceService : ISalesforceService
    {
        private readonly ILogger<SalesForceService> _logger;
        private readonly IDbContextFactory<SyncLogDBContext> _contextFactory;
        private readonly IAuthenticationTokenServices _authenticationTokenServices;

        public SalesForceService(ILogger<SalesForceService> logger, IDbContextFactory<SyncLogDBContext> contextFactory, IAuthenticationTokenServices authenticationTokenServices)
        {
            _logger = logger;
            _contextFactory = contextFactory;
            _authenticationTokenServices = authenticationTokenServices;
        }

        public async Task<HttpResponseMessage> SalesforceApiCall(dynamic entity,string destinationURL)
        {
            string json;

            switch (entity)
            {
                case EventDto e:
                    json = JsonConvert.SerializeObject(new List<EventDto> { entity });
                    break;

                case AttendeeDto a:
                    json = JsonConvert.SerializeObject(new List<AttendeeDto> { entity });
                    break;

                case TicketTypeDto t:
                    json = JsonConvert.SerializeObject(new List<TicketTypeDto> { entity });
                    break;

                case SyncBridge.Domain.Models.CVENT.Contact c:
                    json = JsonConvert.SerializeObject(new List<SyncBridge.Domain.Models.CVENT.Contact> { entity });
                    break;

                case SalesOrderDto s:
                    json = JsonConvert.SerializeObject(new List<SalesOrderDto> { entity });
                    break;

                case ReceiptDto s:
                    json = JsonConvert.SerializeObject(new List<ReceiptDto> { entity });
                    break;



                default:
                    throw new InvalidOperationException($"Unsupported entity type");
            }

            HttpResponseMessage response;

            // Now send json to Salesforce
            using (var client = new HttpClient())
            {
                var accessToken = await _authenticationTokenServices.GetSalesForceAuthToken();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await client.PostAsync(new Uri(destinationURL), content);
            }
            return response;
        }

    }

}
