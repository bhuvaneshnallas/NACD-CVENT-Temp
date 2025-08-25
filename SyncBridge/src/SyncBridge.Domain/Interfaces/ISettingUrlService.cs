using SyncBridge.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncBridge.Domain.Interfaces
{
    public interface ISettingUrlService
    {
        Task<List<Settings>> GetSyncSettings();

        Task<(string sourceUrl, string sourceUpdateUrl, string destinationUrl, string destination_Endpoint_Prefix)> GetSalesforceDestionSourceURL(string moduleName);

        Settings GetMatchRecords(List<Settings> settingsData, string value);


    }
}
