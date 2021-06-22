using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vsWork.Data;
using Fluxor;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using static vsWork.Stores.UserSettingStore;
using vsWork.Services;

namespace vsWork.Stores
{
    public record UserSettingState: BaseSettingState
    {
        public User[] UserList { get; init; }
        public User SelectedUser { get; set; }
        public Organization[] OrganizationList { get; init; }
    }
    public class UserSettingStore
    {
        public class UserSettingFeature : Feature<UserSettingState>
        {
            public override string GetName() => "UserSetting";

            protected override UserSettingState GetInitialState()
            {
                return new UserSettingState
                {
                    Initialized = false,
                    Loading = false,
                    UserList = Array.Empty<User>(),
                    SelectedUser = null,
                    Mode = SettingMode.None,
                    OrganizationList = Array.Empty<Organization>()
                };
            }
        }
        public static class UserSettingReducers
        {
            [ReducerMethod]
            public static UserSettingState OnSetUsers(UserSettingState state, SetUsersAction action)
            {
                return state with
                {
                    UserList = (User[])(action.ListData),
                    Loading = false,
                    Initialized = true,
                    SelectedUser = null
                };
            }
            [ReducerMethod]
            public static UserSettingState OnSetOrganizations(UserSettingState state, SetOrganizationsAction action)
            {
                return state with
                {
                    OrganizationList = (Organization[])(action.ListData),
                };
            }

            [ReducerMethod(typeof(LoadUsersAction))]
            public static UserSettingState OnLoadLoadList(UserSettingState state)
            {
                return state with
                {
                    Loading = true,
                    Mode = SettingMode.None,
                    SelectedUser = null
                };
            }

            [ReducerMethod]
            public static UserSettingState OnUserSettingSetState(UserSettingState state, SetUserStateAction action)
            {
                return action.State;
            }
            [ReducerMethod]
            public static UserSettingState OnSettingBegin(UserSettingState state, UserSettingBeginAction action)
            {
                return state with
                {
                    SelectedUser = action.SelectedData,
                    Mode = action.Mode
                };
            }
            [ReducerMethod]
            public static UserSettingState OnSettingUser(UserSettingState state, UserSettingAction action)
            {
                return state with
                {
                    SelectedUser = action.SelectedData
                };
            }
            [ReducerMethod]
            public static UserSettingState OnSettingUserFailure(UserSettingState state, UserSettingFailureAction action)
            {
                return state with
                {
                    // 未定
                };
            }
        }
    }
    public class UserSettingEffects
    {
        private readonly IState<UserSettingState> SettingState;
        private readonly IRepository<Organization, int> _organizationRepositoryService;
        private readonly UserRepository _userRepositoryService;
        private readonly IState<CurrentUserState> _currentUserState;
        private readonly ILocalStorageService _localStorageService;
        private readonly NavigationManager _navigationManager;
        private const string StatePersistenceName = "UserSettingState";

        public UserSettingEffects
        (IState<UserSettingState> settingState,
        IRepository<Organization, int> organizationRepositoryService,
        IRepository<User, string> userRepositoryService,
        IState<CurrentUserState> currentUserState,
        ILocalStorageService localStorageService,
        NavigationManager navigationManager)
        {
            SettingState = settingState;
            _userRepositoryService = (UserRepository)userRepositoryService;
            _organizationRepositoryService = organizationRepositoryService;
            _currentUserState = currentUserState;
            _localStorageService = localStorageService;
            _navigationManager = navigationManager;
        }

        [EffectMethod(typeof(LoadUsersAction))]
        public async Task LoadListData(IDispatcher dispatcher)
        {
            User[] users;
            Dictionary<int, Organization> organizations = new Dictionary<int, Organization>();
            
            if (_currentUserState.Value.User.Rank == User.RankType.SystemAdmin)
            {
                users = _userRepositoryService.FindAll().ToArray();
                organizations = _organizationRepositoryService.FindAll().ToDictionary(p => p.OrganizationId);
                foreach (User u in users)
                {
                    u.OrganizationName = organizations[u.OrganizationId].OrganizationName;
                }
                dispatcher.Dispatch(new SetUsersAction(users));
                dispatcher.Dispatch(new SetOrganizationsAction(organizations.Values.ToArray()));
                dispatcher.Dispatch(new LoadUsersSuccessAction());
            }
            else if (_currentUserState.Value.User.Rank == User.RankType.OrganizationAdmin)
            {
                users = _userRepositoryService.GetUsersByOrganizationId(_currentUserState.Value.User.OrganizationId).ToArray();
                dispatcher.Dispatch(new SetUsersAction(users));
                dispatcher.Dispatch(new LoadUsersSuccessAction());
            }
            else
            {
                users = new User[] { _currentUserState.Value.User };
                dispatcher.Dispatch(new SetUsersAction(users));
                dispatcher.Dispatch(new LoadUsersSuccessAction());
                dispatcher.Dispatch(new UserSettingBeginAction(_currentUserState.Value.User, SettingMode.Update));
            }

        }
        [EffectMethod(typeof(UserSettingBeginAction))]
        public async Task SettingBegin(IDispatcher dispatcher)
        {
            if (SettingState.Value.Mode == SettingMode.Add |
                SettingState.Value.Mode == SettingMode.Update)
            {
                _navigationManager.NavigateTo("userSetting");
            }
            else if (SettingState.Value.Mode == SettingMode.Delete)
            {
                dispatcher.Dispatch(new UserSettingAction(SettingState.Value.SelectedUser));
            }

        }
        [EffectMethod(typeof(UserSettingAction))]
        public async Task Setting(IDispatcher dispatcher)
        {
            try
            {
                if (SettingState.Value.Mode == SettingMode.Add)
                {
                    _userRepositoryService.Add(SettingState.Value.SelectedUser);
                    dispatcher.Dispatch(new UserSettingSuccessAction());
                }
                else if (SettingState.Value.Mode == SettingMode.Update)
                {
                    _userRepositoryService.Update(SettingState.Value.SelectedUser);
                    dispatcher.Dispatch(new UserSettingSuccessAction());
                }
                else if (SettingState.Value.Mode == SettingMode.Delete)
                {
                    _userRepositoryService.Remove(SettingState.Value.SelectedUser.UserId);
                    dispatcher.Dispatch(new UserSettingSuccessAction());
                }
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new UserSettingFailureAction(ex.Message));
            }
        }
        [EffectMethod(typeof(UserSettingSuccessAction))]
        public async Task SettingSuccess(IDispatcher dispatcher)
        {
            if (SettingState.Value.Mode != SettingMode.None)
            {
                // 自身を更新した場合サインアウトします
                if (SettingState.Value.SelectedUser.UserId == _currentUserState.Value.User.UserId)
                {
                    dispatcher.Dispatch(new SignOutAction());
                    return;
                }
            }
            if (SettingState.Value.Mode == SettingMode.Add |
                SettingState.Value.Mode == SettingMode.Update)
            {
                _navigationManager.NavigateTo("userList");
            }
            dispatcher.Dispatch(new LoadUsersAction());
        }


        [EffectMethod(typeof(SettingUserLoadStateAction))]
        public async Task LoadState(IDispatcher dispatcher)
        {
            try
            {
                var userSettingState = await _localStorageService.GetItemAsync<UserSettingState>(StatePersistenceName);
                if (userSettingState is not null)
                {
                    dispatcher.Dispatch(new SetUserStateAction(userSettingState));
                    dispatcher.Dispatch(new LoadUserStateSuccessAction());
                }
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new LoadUserStateFailureAction(ex.Message));
            }
        }

        [EffectMethod(typeof(ClearUserStateAction))]
        public async Task ClearState(IDispatcher dispatcher)
        {
            try
            {
                await _localStorageService.RemoveItemAsync(StatePersistenceName);
                dispatcher.Dispatch(new SetUserStateAction(new UserSettingState
                {
                    Initialized = false,
                    Loading = false,
                    UserList = Array.Empty<User>(),
                    SelectedUser = null,
                    Mode = SettingMode.None,
                    OrganizationList = Array.Empty<Organization>()
                })); ;
                dispatcher.Dispatch(new ClearUserStateSuccessAction());
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new ClearUserStateFailureAction(ex.Message));
            }
        }
    }

    #region Actions
    public record LoadUsersAction();
    public record LoadUsersSuccessAction();
    public record LoadUsersFailureAction(string ErrorMessage);
    public record SetUsersAction(User[] ListData);
    public record SetOrganizationsAction(Organization[] ListData);

    public record SettingUserLoadStateAction();

    public record SetUserStateAction(UserSettingState State);
    public record LoadUserStateSuccessAction();
    public record LoadUserStateFailureAction(string ErrorMessage);

    public record ClearUserStateAction();
    public record ClearUserStateSuccessAction();
    public record ClearUserStateFailureAction(string ErrorMessage);

    public record UserSettingBeginAction(User SelectedData, SettingMode Mode);
    public record UserSettingAction(User SelectedData);
    public record UserSettingSuccessAction();
    public record UserSettingFailureAction(string ErrorMessage);


    #endregion
}
