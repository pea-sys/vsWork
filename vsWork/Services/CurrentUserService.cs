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
                PunchInTimeStamp = userstate.TimeStamp;
            }
            
        }
        public void SignOut()
        {
            UserId = "";
            UserName = "";
            State = UserState.StateType.None;
            //CircuitId = ""; セッションは生きている
        }
        public void ChangeState(UserState us)
        {
            State = us.State;
            PunchInTimeStamp = us.TimeStamp;
        }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string CircuitId { get; set; }
        public UserState.StateType State { get; set; }
        public DateTime PunchInTimeStamp { get; set; }
    }
}
