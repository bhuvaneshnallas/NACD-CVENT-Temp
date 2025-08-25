using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SyncBridge.Domain.Common;
using SyncBridge.Domain.Interfaces;
using SyncBridge.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SyncBridge.Infrastructure.Services
{
    public class SettingUrlService : ISettingUrlService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SettingUrlService> _logger;
        private readonly IAuthenticationTokenServices _authenticationTokenServices;

        public SettingUrlService(IConfiguration configuration, ILogger<SettingUrlService> logger, IAuthenticationTokenServices authenticationTokenServices)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
            _authenticationTokenServices = authenticationTokenServices;
        }
        public async Task<List<Settings>> GetSyncSettings()
        {
            List<Settings>? result = null;

            var url = _configuration.GetSection("CVENTApiConfig:SettingURL").Value;
            var client = new HttpClient();
            var accessToken = await _authenticationTokenServices.GetCosmosAuthToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var responses = await client.GetAsync(new Uri(url));
            var items = await responses.Content.ReadAsStringAsync();

            CventSettings settings = JsonConvert.DeserializeObject<CventSettings>(items);
            var settingsData = settings?.Data;
            Predicate<Settings> match = delegate (Settings bk) { return bk.Key.StartsWith("Sync."); };
            result = settingsData?.FindAll(match) ?? new List<Settings>();

            return result;
        }

        public Task<(string sourceUrl, string sourceUpdateUrl, string destinationUrl,string destination_Endpoint_Prefix)> GetSalesforceDestionSourceURL(string moduleName)
        {
            var settingsData = GetSyncSettings().Result;

            var source_Endpoint_Prefix = Constants.CVENTKEYPREFIX;
            var destination_Endpoint_Prefix = Constants.SFKEYPREFIX;

            var sourceUrlKey = string.Join(".", Constants.SYNCKEYPREFIX, source_Endpoint_Prefix, Constants.BASEURL);
            var destinationUrlKey = string.Join(".", Constants.SYNCKEYPREFIX, destination_Endpoint_Prefix, Constants.BASEURL);
            var sourcePathKey = string.Join(".", Constants.SYNCKEYPREFIX, source_Endpoint_Prefix, moduleName, Constants.PATHURL);
            var destinationPathKey = string.Join(".", Constants.SYNCKEYPREFIX, destination_Endpoint_Prefix, moduleName, Constants.PATHURL);
            var notificationPathKey = string.Join(".", Constants.SYNCKEYPREFIX, Constants.SFKEYPREFIX, Constants.NOTIFICATION, Constants.PATHURL);
            _logger.LogInformation(@$"sourceUrlKey {sourceUrlKey} destinationUrlKey {destinationUrlKey} sourcePathKey {sourcePathKey} destinationPathKey {destinationPathKey}");
            var updateUrlKey = string.Join(".", Constants.SYNCKEYPREFIX, source_Endpoint_Prefix, moduleName, Constants.UPDATE, Constants.PATHURL);
            var updatePathUrlKey = GetMatchRecords(settingsData, updateUrlKey).Value;
            var sourceBaseUrl = GetMatchRecords(settingsData, sourceUrlKey).Value;
            var sourcePathUrl = GetMatchRecords(settingsData, sourcePathKey).Value;
            var sourceUrl = sourceBaseUrl + sourcePathUrl;
            var updateUrl = sourceBaseUrl + updatePathUrlKey;

            var destinationBaseUrl = GetMatchRecords(settingsData, destinationUrlKey).Value;
            var destinationPathUrl = GetMatchRecords(settingsData, destinationPathKey).Value;
            var destinationUrl = destinationBaseUrl + destinationPathUrl;

            return Task.FromResult((sourceUrl, updateUrl, destinationUrl,destination_Endpoint_Prefix));
        }


        public Settings GetMatchRecords(List<Settings> settingsData, string value)
        {
            Predicate<Settings> match = delegate (Settings bk) { return bk.Key.StartsWith(value); };
            return settingsData.Find(match);
        }
    }
}
