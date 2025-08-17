using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyncBridge.Domain.Models;

namespace SyncBridge.Domain.Interfaces
{
    public interface IMessageService
    {
        public Task PublishEvent(Notification notification);
    }
}
