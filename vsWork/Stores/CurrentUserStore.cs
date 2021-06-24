using Fluxor;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vsWork.Data;

namespace vsWork.Stores
{
    public record CurrentUserState:IState
    {
        private UserState _userState;

        public string CircuitId { get; set; }
        public User User { get; set; }
        
        public UserState UserState 
        {
            get { return _userState; }
            set
            {
                if (_userState != value)
                {
                    this._userState = value;
                    StateHasChanged();
                }
            }
        }
        public event EventHandler StateChanged;


        private void StateHasChanged()
        {
            this.StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public class CurrentUserFeature : Feature<CurrentUserState>
    {
        public override string GetName() => "CurrentUser";

        protected override CurrentUserState GetInitialState()
        {
            return new CurrentUserState
            {
                CircuitId = string.Empty,
                User = new User(),
                UserState = new UserState()
            };
        }
    }
    public static class UserReducers
    {
        [ReducerMethod]
        public static CurrentUserState OnConnect(CurrentUserState state, ConnectAction action)
        {
            return state with
            {
                CircuitId = action.CircuitId
            };
        }
        [ReducerMethod]
        public static CurrentUserState OnSignIn(CurrentUserState state, SignInAction action)
        {
            return state with
            {
                User = action.User
            };
        }
    }
    public class CurrentUserEffects
    {
        private readonly IRepository<Organization, int> _organizationRepository;
        private readonly IState<CurrentUserState> currentUserState;
        private readonly IRepository<User, string> _userRepositoryService;
        private readonly IRepository<Session, string> _sessionRepository;
        private readonly IRepository<UserState, string> _userStateRepository;
        private readonly IRepository<Attendance, string> _attendanceRepository;

        private readonly NavigationManager _navigationManager;
        private const string StatePersistenceName = "CurrentUserState";

        public CurrentUserEffects
        (IState<CurrentUserState> state,
        IRepository<Organization, int> organizationRepositoryService,
        IRepository<User, string> userRepositoryService,
        IRepository<Session, string> sessionRepositoryService,
        IRepository<UserState, string> userStateRepositoryService,
        IRepository<Attendance, string> attendanceRepositoryService,
        NavigationManager navigationManager)
        {
            currentUserState = state;
            _userRepositoryService = userRepositoryService;
            _sessionRepository = sessionRepositoryService;
            _userStateRepository = userStateRepositoryService;
            _attendanceRepository = attendanceRepositoryService;
            _organizationRepository = organizationRepositoryService;
            _navigationManager = navigationManager;
        }
        [EffectMethod(typeof(DisConnectAction))]
        public async Task DisConnect(IDispatcher dispatcher)
        {
            // 認証済みセッション情報の削除
            if (!string.IsNullOrEmpty(currentUserState.Value.User.UserId))
            {
                _sessionRepository.Remove(currentUserState.Value.CircuitId);
                currentUserState.Value.CircuitId = string.Empty;
                currentUserState.Value.User = new User();
                currentUserState.Value.UserState = new UserState();
            }

        }
        [EffectMethod(typeof(SignInAction))]
        public async Task SignIn(IDispatcher dispatcher)
        {
            // Validatorで認証済み
            currentUserState.Value.User = _userRepositoryService.FindById(currentUserState.Value.User.UserId);
            _sessionRepository.Add(new Session() { SessionId = currentUserState.Value.CircuitId, UserId = currentUserState.Value.User.UserId });
            currentUserState.Value.UserState = _userStateRepository.FindById(currentUserState.Value.User.UserId);
            if (currentUserState.Value.UserState is null)
            {
                currentUserState.Value.UserState = new UserState{ UserId = currentUserState.Value.User.UserId };
            }

            _navigationManager.NavigateTo("summary");
        }
        [EffectMethod(typeof(SignOutAction))]
        public async Task SignOut(IDispatcher dispatcher)
        {
            // セッションは生きているのでCurcuitIdはそのまま
            _sessionRepository.Remove(currentUserState.Value.CircuitId);
            currentUserState.Value.User = new User();
            currentUserState.Value.UserState = new UserState();
            _navigationManager.NavigateTo("");
        }
        [EffectMethod(typeof(PunchInAction))]
        public async Task PunchIn(IDispatcher dispatcher)
        {
            _attendanceRepository.Add(new Attendance() { UserId = currentUserState.Value.User.UserId });
            currentUserState.Value.UserState = _userStateRepository.FindById(currentUserState.Value.User.UserId);
        }
        [EffectMethod(typeof(PunchOutAction))]
        public async Task PunchOut(IDispatcher dispatcher)
        {
            ((AttendanceRepository)_attendanceRepository).UpdateAtPunchOutTimestamp(new Attendance() { UserId = currentUserState.Value.User.UserId });
            currentUserState.Value.UserState = _userStateRepository.FindById(currentUserState.Value.User.UserId);
        }
    }
    #region Actions
    public record ConnectAction(string CircuitId);
    public record DisConnectAction(string CircuitId);
    public record SignInAction(User User);
    public record SignOutAction();
    public record PunchInAction();
    public record PunchOutAction();
    #endregion
}
