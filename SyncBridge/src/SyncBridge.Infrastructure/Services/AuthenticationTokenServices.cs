using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SyncBridge.Domain.Common;
using SyncBridge.Domain.DTOs;
using SyncBridge.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SyncBridge.Infrastructure.Services
{
    public class AuthenticationTokenServices : IAuthenticationTokenServices
    {
        private readonly IConfiguration _configuration;

        public AuthenticationTokenServices(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<string> GetCosmosAuthToken(bool requestFromQueue = false)
        {
            string accessToken = string.Empty;

            var url = _configuration["NacdApiConfig:TokenApi"];

            using (var client = new HttpClient())
            {
                var reqContent = new Dictionary<string, string>();

                if (!requestFromQueue)
                {
                    reqContent.Add("scope", _configuration["NacdApiConfig:scope"] ?? string.Empty);
                    reqContent.Add("client_id", _configuration["NacdApiConfig:client_id"] ?? string.Empty);
                    reqContent.Add("client_secret", _configuration["NacdApiConfig:client_secret"] ?? string.Empty);
                }
                else
                {
                    reqContent.Add("scope", _configuration["NacdApiConfig:functionscope"] ?? string.Empty);
                    reqContent.Add("client_id", _configuration["NacdApiConfig:function_client_id"] ?? string.Empty);
                    reqContent.Add("client_secret", _configuration["NacdApiConfig:function_client_secret"] ?? string.Empty);
                }

                reqContent.Add("grant_type", _configuration["NacdApiConfig:grant_type"] ?? string.Empty);

                using (var content = new FormUrlEncodedContent(reqContent))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                    {
                        CharSet = "UTF-8"
                    };

                    client.DefaultRequestHeaders.ExpectContinue = false;

                    var response = await client.PostAsync(new Uri(url), content);
                    var tokenResponse = await response.Content.ReadAsStringAsync();

                    var jsonResponse = JsonConvert.DeserializeObject<NacdTokenResponse>(tokenResponse);

                    if (jsonResponse is not null && !string.IsNullOrEmpty(jsonResponse.Access_token))
                    {
                        accessToken = jsonResponse.Access_token;
                    }
                }
            }

            return accessToken;
        }


        public async Task<string> GetSalesForceAuthToken()
        {
            string accessToken = string.Empty;

            var url = _configuration["SalesforceApiConfig:TokenApi"];

            using (var client = new HttpClient())
            {
                var reqContent = new Dictionary<string, string>
                {
                    { "username",      _configuration["SalesforceApiConfig:username"]      ?? string.Empty },
                    { "password",      _configuration["SalesforceApiConfig:password"]      ?? string.Empty },
                    { "grant_type",    _configuration["SalesforceApiConfig:grant_type"]    ?? string.Empty },
                    { "client_id",     _configuration["SalesforceApiConfig:client_id"]     ?? string.Empty },
                    { "client_secret", _configuration["SalesforceApiConfig:client_secret"] ?? string.Empty }
                };

                using (var content = new FormUrlEncodedContent(reqContent))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                    {
                        CharSet = "UTF-8"
                    };

                    client.DefaultRequestHeaders.ExpectContinue = false;

                    var response = await client.PostAsync(new Uri(url), content);
                    var tokenResponse = await response.Content.ReadAsStringAsync();

                    var jsonResponse = JsonConvert.DeserializeObject<SalesForceTokenResponse>(tokenResponse);

                    if (jsonResponse is not null && !string.IsNullOrEmpty(jsonResponse.Access_token))
                    {
                        accessToken = jsonResponse.Access_token;
                    }
                }
            }

            return accessToken;
        }
    }
}
