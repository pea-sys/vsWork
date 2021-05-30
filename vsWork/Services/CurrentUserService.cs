using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vsWork.Data;

namespace vsWork.Services
{
    /// <summary>
    /// 現在のユーザ情報を提供するサービス
    /// データを保持するだけで処理は記述しない方針
    /// </summary>
    public class CurrentUserService
    {
        public void SignIn(CircuitHandlerService circuitHandlerServive, User user, UserState userstate)
        {
            UserId = user.UserId;
            UserName = user.UserName;
            CircuitId = circuitHandlerServive.CircuitId;
            if (userstate is null)
            {
                State = UserState.StateType.None;
            }
            else
            {
                State = userstate.State;
            }
        }
        public void SignOut()
        {
            UserId = "";
            UserName = "";
            //CircuitId = ""; セッションは生きている
        }
        public void PunchIn()
        {
            State = UserState.StateType.PunchIn;
        }
        public void PunchOut()
        {
            State = UserState.StateType.PunchOut;
        }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string CircuitId { get; set; }
        public UserState.StateType State { get; set; }
    }
}
