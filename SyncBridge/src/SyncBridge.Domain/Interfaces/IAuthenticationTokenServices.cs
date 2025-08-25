using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncBridge.Domain.Interfaces
{
    public interface IAuthenticationTokenServices
    {
        Task<string> GetCosmosAuthToken(bool requestFromQueue = false);
        Task<string> GetSalesForceAuthToken();
    }
}
