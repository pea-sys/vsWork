using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vsWork.Data;

namespace vsWork.Services
{
    public interface IUserOnlineService
    {
        void Connect(string circuitId, User user);
        void DisConnect(string circuitId);
    }
}
