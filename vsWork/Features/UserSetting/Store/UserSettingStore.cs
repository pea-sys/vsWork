using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vsWork.Data;
using Fluxor;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace vsWork.Features.UserSetting.Store
{
    public record UserSettingState
    {
        public bool Initialized { get; init; }
        public bool Loading { get; init; }
        public User[] Users { get; init; }
        public User SelectedUser { get; set; }
        public UserSettingMode Mode { get; set; }
    }
    public enum UserSettingMode
    {
        None,
        Add,
        Update,
        Delete
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
                    Users = Array.Empty<User>(),
                    SelectedUser = null,
                    Mode = UserSettingMode.None
                };
            }
        }
        public static class UserSettingReducers
        {
            [ReducerMethod]
            public static UserSettingState OnSetUsers(UserSettingState state, UserSettingSetUsersAction action)
            {
                return state with
                {
                    Users = action.Users,
                    Loading = false,
                    Initialized = true,
                    SelectedUser = null
                };
            }

            [ReducerMethod(typeof(UserSettingLoadUsersAction))]
            public static UserSettingState OnLoadUsers(UserSettingState state)
            {
                return state with
                {
                    Loading = true,
                    Mode = UserSettingMode.None
                };
            }

            [ReducerMethod]
            public static UserSettingState OnUserSettingSetState(UserSettingState state, UserSettingSetStateAction action)
            {
                return action.UserSettingState;
            }
            [ReducerMethod]
            public static UserSettingState OnSettingUserBegin(UserSettingState state , UserSettingSettingUserBeginAction action)
            {
                return state with
                {
                    SelectedUser = action.User,
                    Mode = action.Mode
                };
            }
            [ReducerMethod]
            public static UserSettingState OnSettingUser(UserSettingState state , UserSettingSettingUserAction action)
            {
                return state with
                {
                    SelectedUser = action.User
                };
            }
            [ReducerMethod]
            public static UserSettingState OnSettingUserSuccess(UserSettingState state, UserSettingSettingUserSuccessAction action)
            {
                return state with
                {
                    SelectedUser = null,
                };
            }
            [ReducerMethod]
            public static UserSettingState OnSettingUserFailure(UserSettingState state, UserSettingSettingUserFailureAction action)
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

        private readonly IState<UserSettingState> UserSettingState;
        private readonly IRepository<User,string> _userRepositoryService;
        private readonly ILocalStorageService _localStorageService;
        private readonly NavigationManager _navigationManager;
        private const string UserSettingStatePersistenceName = "UserSettingState";

        public UserSettingEffects
        (IState<UserSettingState> userSettingState,
        IRepository<User,string> userRepositoryService,
        ILocalStorageService localStorageService,
        NavigationManager navigationManager)
        {
            UserSettingState = userSettingState;
            _userRepositoryService = userRepositoryService;
            _localStorageService = localStorageService;
            _navigationManager = navigationManager;
        }

        [EffectMethod(typeof(UserSettingLoadUsersAction))]
        public async Task LoadUsers(IDispatcher dispatcher)
        {
            User[] users = _userRepositoryService.FindAll().ToArray();
            dispatcher.Dispatch(new UserSettingSetUsersAction(users));
            dispatcher.Dispatch(new UserSettingLoadUsersSuccessAction());            
        }
        [EffectMethod(typeof(UserSettingSettingUserBeginAction))]
        public async Task SettingUserBegin(IDispatcher dispatcher)
        {
            if (UserSettingState.Value.Mode == UserSettingMode.Add |
                UserSettingState.Value.Mode == UserSettingMode.Update)
            {
                _navigationManager.NavigateTo("userSetting");
            }
            else
            {
                dispatcher.Dispatch(new UserSettingSettingUserAction(UserSettingState.Value.SelectedUser));
            }
            
        }
        [EffectMethod(typeof(UserSettingSettingUserAction))]
        public async Task SettingUser(IDispatcher dispatcher)
        {
            try
            {
                if (UserSettingState.Value.Mode == UserSettingMode.Add)
                {
                    _userRepositoryService.Add(UserSettingState.Value.SelectedUser);
                    dispatcher.Dispatch(new UserSettingSettingUserSuccessAction());
                }
                else if (UserSettingState.Value.Mode == UserSettingMode.Update)
                {
                    _userRepositoryService.Update(UserSettingState.Value.SelectedUser);
                    dispatcher.Dispatch(new UserSettingSettingUserSuccessAction());
                }
                else if (UserSettingState.Value.Mode == UserSettingMode.Delete)
                {
                    _userRepositoryService.Remove(UserSettingState.Value.SelectedUser.UserId);
                    dispatcher.Dispatch(new UserSettingSettingUserSuccessAction());
                }
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new UserSettingSettingUserFailureAction(ex.Message));
            }
        }
        [EffectMethod(typeof(UserSettingSettingUserSuccessAction))]
        public async Task SettingSuccess(IDispatcher dispatcher)
        {
            if (UserSettingState.Value.Mode == UserSettingMode.Add |
                UserSettingState.Value.Mode == UserSettingMode.Update)
            {
                _navigationManager.NavigateTo("userList");
            }
            dispatcher.Dispatch(new UserSettingLoadUsersAction());
        }

        [EffectMethod]
        public async Task PersistState(UserSettingStateAction action, IDispatcher dispatcher)
        {
            try
            {
                await _localStorageService.SetItemAsync(UserSettingStatePersistenceName, action.UserSettingState);
                dispatcher.Dispatch(new UserSettingPersistStateSuccessAction());
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new UserSettingPersistStateFailureAction(ex.Message));
            }
        }

        [EffectMethod(typeof(UserSettingLoadStateAction))]
        public async Task LoadState(IDispatcher dispatcher)
        {
            try
            {
                var userSettingState = await _localStorageService.GetItemAsync<UserSettingState>(UserSettingStatePersistenceName);
                if (userSettingState is not null)
                {
                    dispatcher.Dispatch(new UserSettingSetStateAction(userSettingState));
                    dispatcher.Dispatch(new UserSettingLoadStateSuccessAction());
                }
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new UserSettingLoadStateFailureAction(ex.Message));
            }
        }

        [EffectMethod(typeof(UserSettingClearStateAction))]
        public async Task ClearState(IDispatcher dispatcher)
        {
            try
            {
                await _localStorageService.RemoveItemAsync(UserSettingStatePersistenceName);
                dispatcher.Dispatch(new UserSettingSetStateAction(new UserSettingState
                {
                    Initialized = false,
                    Loading = false,
                    Users = Array.Empty<User>(),
                    Mode = UserSettingMode.None
                    
                }));
                dispatcher.Dispatch(new UserSettingClearStateSuccessAction());
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new UserSettingClearStateFailureAction(ex.Message));
            }
        }
    }

    #region Actions
    public record UserSettingLoadUsersAction();
    public record UserSettingLoadUsersSuccessAction();
    public record UserSettingSetUsersAction(User[] Users);
    public record UserSettingSetStateAction(UserSettingState UserSettingState);
    public record UserSettingLoadStateAction();
    public record UserSettingLoadStateSuccessAction();
    public record UserSettingLoadStateFailureAction(string ErrorMessage);
    public record UserSettingStateAction(UserSettingState UserSettingState);

    public record UserSettingPersistStateAction(UserSettingState UserSettingState);
    public record UserSettingPersistStateSuccessAction();
    public record UserSettingPersistStateFailureAction(string ErrorMessage);
    public record UserSettingClearStateAction();
    public record UserSettingClearStateSuccessAction();
    public record UserSettingClearStateFailureAction(string ErrorMessage);

    public record UserSettingSettingUserBeginAction(User User, UserSettingMode Mode);
    public record UserSettingSettingUserAction(User User);
    public record UserSettingSettingUserSuccessAction();
    public record UserSettingSettingUserFailureAction(string ErrorMessage);
    #endregion
}
