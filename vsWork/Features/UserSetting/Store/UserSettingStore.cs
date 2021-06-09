using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vsWork.Data;
using Fluxor;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using vsWork.Features.Shared.Store;

namespace vsWork.Features.UserSetting.Store
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
            public static SettingState<User> OnSetUsers(SettingState<User> state, SetListDataAction action)
            {
                return state with
                {
                    ListData = (User[])(action.ListData),
                    Loading = false,
                    Initialized = true,
                    SelectedData = null
                };
            }

            [ReducerMethod(typeof(LoadListDataAction))]
            public static SettingState<IEntity> OnLoadLoadList(SettingState<IEntity> state)
            {
                return state with
                {
                    Loading = true,
                    Mode = SettingMode.None
                };
            }

            [ReducerMethod]
            public static SettingState<User> OnUserSettingSetState(SettingState<User> state, SetStateAction action)
            {
                return action.State;
            }
            [ReducerMethod]
            public static SettingState<User> OnSettingBegin(SettingState<User> state , SettingBeginAction action)
            {
                return state with
                {
                    SelectedData = action.SelectedData,
                    Mode = action.Mode
                };
            }
            [ReducerMethod]
            public static SettingState<IEntity> OnSettingUser(SettingState<IEntity> state , SettingAction action)
            {
                return state with
                {
                    SelectedData = action.SelectedData
                };
            }
            [ReducerMethod]
            public static SettingState<IEntity> OnSettingUserSuccess(SettingState<IEntity> state, SettingSuccessAction action)
            {
                return state with
                {
                    SelectedData = null,
                };
            }
            [ReducerMethod]
            public static SettingState<IEntity> OnSettingUserFailure(SettingState<IEntity> state, SettingFailureAction action)
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
        private readonly IRepository<User,string> _userRepositoryService;
        private readonly ILocalStorageService _localStorageService;
        private readonly NavigationManager _navigationManager;
        private const string StatePersistenceName = "UserSettingState";

        public UserSettingEffects
        (IState<SettingState<User>> settingState,
        IRepository<User,string> userRepositoryService,
        ILocalStorageService localStorageService,
        NavigationManager navigationManager)
        {
            SettingState = settingState;
            _userRepositoryService = userRepositoryService;
            _localStorageService = localStorageService;
            _navigationManager = navigationManager;
        }

        [EffectMethod(typeof(LoadListDataAction))]
        public async Task LoadListData(IDispatcher dispatcher)
        {
            User[] users = _userRepositoryService.FindAll().ToArray();
            dispatcher.Dispatch(new SetListDataAction(users));
            dispatcher.Dispatch(new LoadListDataSuccessAction());            
        }
        [EffectMethod(typeof(SettingBeginAction))]
        public async Task SettingBegin(IDispatcher dispatcher)
        {
            if (SettingState.Value.Mode == SettingMode.Add |
                SettingState.Value.Mode == SettingMode.Update)
            {
                _navigationManager.NavigateTo("userSetting");
            }
            else if (SettingState.Value.Mode == SettingMode.Delete)
            {
                dispatcher.Dispatch(new SettingAction(SettingState.Value.SelectedData));
            }
            
        }
        [EffectMethod(typeof(SettingAction))]
        public async Task Setting(IDispatcher dispatcher)
        {
            try
            {
                if (SettingState.Value.Mode == SettingMode.Add)
                {
                    _userRepositoryService.Add(SettingState.Value.SelectedData);
                    dispatcher.Dispatch(new SettingSuccessAction());
                }
                else if (SettingState.Value.Mode == SettingMode.Update)
                {
                    _userRepositoryService.Update(SettingState.Value.SelectedData);
                    dispatcher.Dispatch(new SettingSuccessAction());
                }
                else if (SettingState.Value.Mode == SettingMode.Delete)
                {
                    _userRepositoryService.Remove(SettingState.Value.SelectedData.UserId);
                    dispatcher.Dispatch(new SettingSuccessAction());
                }
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new SettingFailureAction(ex.Message));
            }
        }
        [EffectMethod(typeof(SettingSuccessAction))]
        public async Task SettingSuccess(IDispatcher dispatcher)
        {
            if (SettingState.Value.Mode == SettingMode.Add |
                SettingState.Value.Mode == SettingMode.Update)
            {
                _navigationManager.NavigateTo("userList");
            }
            dispatcher.Dispatch(new LoadListDataAction());
        }

        [EffectMethod]
        public async Task PersistState(StateAction action, IDispatcher dispatcher)
        {
            try
            {
                await _localStorageService.SetItemAsync(StatePersistenceName, action.State);
                dispatcher.Dispatch(new PersistStateSuccessAction());
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new PersistStateFailureAction(ex.Message));
            }
        }

        [EffectMethod(typeof(LoadStateAction))]
        public async Task LoadState(IDispatcher dispatcher)
        {
            try
            {
                var userSettingState = await _localStorageService.GetItemAsync<SettingState<User>>(StatePersistenceName);
                if (userSettingState is not null)
                {
                    dispatcher.Dispatch(new SetStateAction(userSettingState));
                    dispatcher.Dispatch(new LoadStateSuccessAction());
                }
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new LoadStateFailureAction(ex.Message));
            }
        }

        [EffectMethod(typeof(ClearStateAction))]
        public async Task ClearState(IDispatcher dispatcher)
        {
            try
            {
                await _localStorageService.RemoveItemAsync(StatePersistenceName);
                dispatcher.Dispatch(new SetStateAction(new SettingState<User>
                {
                    Initialized = false,
                    Loading = false,
                    ListData = Array.Empty<User>(),
                    SelectedData = null,
                    Mode = SettingMode.None

                })); ;
                dispatcher.Dispatch(new ClearStateSuccessAction());
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new ClearStateFailureAction(ex.Message));
            }
        }
    }

    #region Actions
    public record LoadListDataAction();
    public record LoadListDataSuccessAction();
    public record SetListDataAction(User[] ListData);
    public record SetStateAction(SettingState<User> State);
    public record LoadStateAction();
    public record LoadStateSuccessAction();
    public record LoadStateFailureAction(string ErrorMessage);
    public record StateAction(SettingState<User> State);

    public record PersistStateAction(SettingState<User> State);
    public record PersistStateSuccessAction();
    public record PersistStateFailureAction(string ErrorMessage);
    public record ClearStateAction();
    public record ClearStateSuccessAction();
    public record ClearStateFailureAction(string ErrorMessage);

    public record SettingBeginAction(User SelectedData, SettingMode Mode);
    public record SettingAction(User SelectedData);
    public record SettingSuccessAction();
    public record SettingFailureAction(string ErrorMessage);
    #endregion
}
