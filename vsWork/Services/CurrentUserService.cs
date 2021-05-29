using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vsWork.Data;

namespace vsWork.Services
{
    public class CurrentUserService
    {
    public void SignIn(CircuitHandlerService circuitHandlerServive, User user)
    {
            UserId = user.UserId;
            UserName = user.UserName;
            CircuitId = circuitHandlerServive.CircuitId;
    }
    public void SignOut()
    {
            UserId = "";
            UserName = "";
            //CircuitId = ""; セッションは生きている
    }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string CircuitId { get; set; }
    }
}
