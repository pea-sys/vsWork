using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vsWork.Data;
using Fluxor;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using vsWork.Features.Shared.Store;

namespace vsWork.Features.Users.Store
{
    public class UserSettingStore
    {
        public class UserSettingFeature : Feature<SettingState<User>>
        {
            public override string GetName() => "UserSetting";

            protected override SettingState<User> GetInitialState()
            {
                return new SettingState<User>
                {
                    Initialized = false,
                    Loading = false,
                    ListData = Array.Empty<User>(),
                    SelectedData = null,
                    Mode = SettingMode.None
                };
            }
        }
        public static class UserSettingReducers
        {
            [ReducerMethod]
            public static SettingState<User> OnSetUsers(SettingState<User> state, SetUsersAction action)
            {
                return state with
                {
                    ListData = (User[])(action.ListData),
                    Loading = false,
                    Initialized = true,
                    SelectedData = null
                };
            }

            [ReducerMethod(typeof(LoadUsersAction))]
            public static SettingState<IEntity> OnLoadLoadList(SettingState<IEntity> state)
            {
                return state with
                {
                    Loading = true,
                    Mode = SettingMode.None
                };
            }

            [ReducerMethod]
            public static SettingState<User> OnUserSettingSetState(SettingState<User> state, SetUserStateAction action)
            {
                return action.State;
            }
            [ReducerMethod]
            public static SettingState<User> OnSettingBegin(SettingState<User> state, UserSettingBeginAction action)
            {
                return state with
                {
                    SelectedData = action.SelectedData,
                    Mode = action.Mode
                };
            }
            [ReducerMethod]
            public static SettingState<IEntity> OnSettingUser(SettingState<IEntity> state, UserSettingAction action)
            {
                return state with
                {
                    SelectedData = action.SelectedData
                };
            }
            [ReducerMethod]
            public static SettingState<IEntity> OnSettingUserSuccess(SettingState<IEntity> state, UserSettingSuccessAction action)
            {
                return state with
                {
                    SelectedData = null,
                };
            }
            [ReducerMethod]
            public static SettingState<IEntity> OnSettingUserFailure(SettingState<IEntity> state, UserSettingFailureAction action)
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

        private readonly IState<SettingState<User>> SettingState;
        private readonly IRepository<User, string> _userRepositoryService;
        private readonly ILocalStorageService _localStorageService;
        private readonly NavigationManager _navigationManager;
        private const string StatePersistenceName = "UserSettingState";

        public UserSettingEffects
        (IState<SettingState<User>> settingState,
        IRepository<User, string> userRepositoryService,
        ILocalStorageService localStorageService,
        NavigationManager navigationManager)
        {
            SettingState = settingState;
            _userRepositoryService = userRepositoryService;
            _localStorageService = localStorageService;
            _navigationManager = navigationManager;
        }

        [EffectMethod(typeof(LoadUsersAction))]
        public async Task LoadListData(IDispatcher dispatcher)
        {
            User[] users = _userRepositoryService.FindAll().ToArray();
            dispatcher.Dispatch(new SetUsersAction(users));
            dispatcher.Dispatch(new LoadUsersSuccessAction());
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
                dispatcher.Dispatch(new UserSettingAction(SettingState.Value.SelectedData));
            }

        }
        [EffectMethod(typeof(UserSettingAction))]
        public async Task Setting(IDispatcher dispatcher)
        {
            try
            {
                if (SettingState.Value.Mode == SettingMode.Add)
                {
                    _userRepositoryService.Add(SettingState.Value.SelectedData);
                    dispatcher.Dispatch(new UserSettingSuccessAction());
                }
                else if (SettingState.Value.Mode == SettingMode.Update)
                {
                    _userRepositoryService.Update(SettingState.Value.SelectedData);
                    dispatcher.Dispatch(new UserSettingSuccessAction());
                }
                else if (SettingState.Value.Mode == SettingMode.Delete)
                {
                    _userRepositoryService.Remove(SettingState.Value.SelectedData.UserId);
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
                var userSettingState = await _localStorageService.GetItemAsync<SettingState<User>>(StatePersistenceName);
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
                dispatcher.Dispatch(new SetUserStateAction(new SettingState<User>
                {
                    Initialized = false,
                    Loading = false,
                    ListData = Array.Empty<User>(),
                    SelectedData = null,
                    Mode = SettingMode.None

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
    public record SetUsersAction(User[] ListData);

    public record SettingUserLoadStateAction();

    public record SetUserStateAction(SettingState<User> State);
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
