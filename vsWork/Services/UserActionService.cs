using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vsWork.Data;

namespace vsWork.Services
{
    /// <summary>
    /// ユーザーアクションを提供するサービス
    /// </summary>
    public class UserActionService
    {
        /// <summary>
        /// セッションID
        /// </summary>
        private string circuitId;
        /// <summary>
        /// ユーザリポジトリサービス
        /// </summary>
        private readonly UserRepository userRepository;
        /// <summary>
        /// セッションリポジトリサービス
        /// </summary>
        private readonly SessionRepository sessionRepository;
        /// <summary>
        /// ユーザ状態リポジトリサービス
        /// </summary>
        private readonly UserStateRepository userStateRepository;
        /// <summary>
        /// 打刻レポジトリ
        /// </summary>
        private readonly AttendanceRepository attendanceRepository;
        /// <summary>
        /// 任意のイベントを実行するイベントハンドラ
        /// </summary>
        public event EventHandler StateChanged;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UserActionService(
        IRepository<User, string> userRepository,
        IRepository<Session, string> sessionRepository,
        IRepository<UserState, string> userStateRepository,
        IRepository<Attendance, string> attendanceRepository)
        {
            this.userRepository = (UserRepository)userRepository;
            this.sessionRepository = (SessionRepository)sessionRepository;
            this.userStateRepository = (UserStateRepository)userStateRepository;
            this.attendanceRepository = (AttendanceRepository)attendanceRepository;
        }
        /// <summary>
        /// セッション開始
        /// </summary>
        /// <param name="circuitId">セッションID</param>
        public void Connect(string circuitId)
        {
            this.circuitId = circuitId;
        }
        /// <summary>
        /// セッション終了
        /// </summary>
        /// <param name="circuitId">セッションID</param>
        public void DisConnect(string circuitId)
        {
            // 認証済みセッションを破棄する
            if (!string.IsNullOrEmpty(UserId))
            {
                sessionRepository.Remove(circuitId);
                this.circuitId = string.Empty;
            }
        }
        /// <summary>
        /// サインイン
        /// </summary>
        /// <param name="user">認証情報</param>
        /// <param name="userstate"></param>
        public void SignIn(string userId)
        {
            User user = userRepository.FindById(userId);
            sessionRepository.Add(new Session() { SessionId = circuitId, UserId = user.UserId });
            UserState userState = userStateRepository.FindById(user.UserId);

            UserId = user.UserId;
            UserName = user.UserName;

            if (userState is null)
            {
                State = UserState.StateType.None;
            }
            else
            {
                State = userState.State;
                PunchInTimeStamp = userState.TimeStamp;
            }
        }
        /// <summary>
        /// サインアウト
        /// ※セッションは生きているのでcircuitIdは残します
        /// </summary>
        public void SignOut()
        {
            sessionRepository.Remove(circuitId);
            UserId = "";
            UserName = "";
            State = UserState.StateType.None;
        }
        /// <summary>
        /// 出勤打刻
        /// </summary>
        public void PunchIn()
        {
            attendanceRepository.Add(new Attendance() { UserId = UserId });
            UserState us = userStateRepository.FindById(UserId);
            State = us.State;
            PunchInTimeStamp = us.TimeStamp;
        }
        /// <summary>
        /// 退勤打刻
        /// </summary>
        public void PunchOut()
        {
            attendanceRepository.UpdateAtPunchOutTimestamp(new Attendance() { UserId = UserId });
            UserState us = userStateRepository.FindById(UserId);
            State = us.State;
            PunchInTimeStamp = us.TimeStamp;
        }
        /// <summary>
        /// イベントを実行する
        /// </summary>
        private void StateHasChanged()
        {
            this.StateChanged?.Invoke(this, EventArgs.Empty);
        }
        public string UserId { get; private set; }
        public string UserName { get; private set; }
        private UserState.StateType _state = UserState.StateType.None;
        public UserState.StateType State
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    this._state = value;
                    StateHasChanged();
                }
            }
        }
        public DateTime PunchInTimeStamp { get; private set; }
    }
}
