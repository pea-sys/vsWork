using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vsWork.Data;
using Fluxor;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace vsWork.Stores
{
    public record OrganizationSettingState: BaseSettingState
    {
        public Organization[] OrganizationList { get; init; }
        public Organization SelectedOrganization { get; set; }
    }
    public class OrganizationSettingStore
    {
        public class OrganizationSettingFeature : Feature<OrganizationSettingState>
        {
            public override string GetName() => "OrganizationSetting";

            protected override OrganizationSettingState GetInitialState()
            {
                return new OrganizationSettingState
                {
                    Initialized = false,
                    Loading = false,
                    OrganizationList = Array.Empty<Organization>(),
                    SelectedOrganization = null,
                    Mode = SettingMode.None
                };
            }
        }
        public static class OrganizationSettingReducers
        {
            [ReducerMethod]
            public static OrganizationSettingState OnSetOrganizations(OrganizationSettingState state, SetOrganizationsAction action)
            {
                return state with
                {
                    OrganizationList = (Organization[])(action.ListData),
                    Loading = false,
                    Initialized = true,
                    SelectedOrganization = null
                };
            }

            [ReducerMethod(typeof(LoadOrganizationsAction))]
            public static OrganizationSettingState OnLoadLoadList(OrganizationSettingState state)
            {
                return state with
                {
                    Loading = true,
                    Mode = SettingMode.None
                };
            }

            [ReducerMethod]
            public static OrganizationSettingState OnOrganizationSettingSetState(OrganizationSettingState state, SetOrganizationStateAction action)
            {
                return action.State;
            }
            [ReducerMethod]
            public static OrganizationSettingState OnSettingBegin(OrganizationSettingState state, OrganizationSettingBeginAction action)
            {
                return state with
                {
                    SelectedOrganization = action.SelectedData,
                    Mode = action.Mode
                };
            }
            [ReducerMethod]
            public static OrganizationSettingState OnSettingOrganization(OrganizationSettingState state, OrganizationSettingAction action)
            {
                return state with
                {
                    SelectedOrganization = action.SelectedData
                };
            }
            [ReducerMethod]
            public static OrganizationSettingState OnSettingOrganizationSuccess(OrganizationSettingState state, OrganizationSettingSuccessAction action)
            {
                return state with
                {
                    SelectedOrganization = null,
                };
            }
        }
    }
    public class OrganizationSettingEffects
    {

        private readonly IState<OrganizationSettingState> SettingState;
        private readonly IRepository<Organization, int> _organizationRepositoryService;
        private readonly ILocalStorageService _localStorageService;
        private readonly IState<CurrentUserState> CurrentState;
        private readonly NavigationManager _navigationManager;
        private const string StatePersistenceName = "OrganizationSettingState";

        public OrganizationSettingEffects
        (IState<OrganizationSettingState> settingState,
        IState<CurrentUserState> currentState,
        IRepository<Organization, int> organizationRepositoryService,
        ILocalStorageService localStorageService,
        NavigationManager navigationManager)
        {
            SettingState = settingState;
            CurrentState = currentState;
            _organizationRepositoryService = organizationRepositoryService;
            _localStorageService = localStorageService;
            _navigationManager = navigationManager;
        }

        [EffectMethod(typeof(LoadOrganizationsAction))]
        public async Task LoadListData(IDispatcher dispatcher)
        {
            Organization[] organizations = _organizationRepositoryService.FindAll().ToArray();
            dispatcher.Dispatch(new SetOrganizationsAction(organizations));
            dispatcher.Dispatch(new LoadOrganizationsSuccessAction());
        }
        [EffectMethod(typeof(OrganizationSettingBeginAction))]
        public async Task SettingBegin(IDispatcher dispatcher)
        {
            if (SettingState.Value.Mode == SettingMode.Add |
                SettingState.Value.Mode == SettingMode.Update)
            {
                _navigationManager.NavigateTo("organizationSetting");
            }
            else if (SettingState.Value.Mode == SettingMode.Delete)
            {
                dispatcher.Dispatch(new OrganizationSettingAction(SettingState.Value.SelectedOrganization));
            }
        }
        [EffectMethod(typeof(OrganizationSettingAction))]
        public async Task Setting(IDispatcher dispatcher)
        {
            try
            {
                if (SettingState.Value.Mode == SettingMode.Add)
                {
                    _organizationRepositoryService.Add(SettingState.Value.SelectedOrganization);

                    dispatcher.Dispatch(new OrganizationSettingSuccessAction());
                }
                else if (SettingState.Value.Mode == SettingMode.Update)
                {
                    _organizationRepositoryService.Update(SettingState.Value.SelectedOrganization);
                    dispatcher.Dispatch(new OrganizationSettingSuccessAction());
                }
                else if (SettingState.Value.Mode == SettingMode.Delete)
                {
                    _organizationRepositoryService.Remove(SettingState.Value.SelectedOrganization.OrganizationId);
                    dispatcher.Dispatch(new OrganizationSettingSuccessAction());
                }
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new OrganizationSettingFailureAction(ex.Message));
            }
        }
        [EffectMethod(typeof(OrganizationSettingSuccessAction))]
        public async Task SettingSuccess(IDispatcher dispatcher)
        {
            CurrentState.Value.Organization = _organizationRepositoryService.FindById(CurrentState.Value.User.OrganizationId);

            if (SettingState.Value.Mode == SettingMode.Add |
                SettingState.Value.Mode == SettingMode.Update)
            {
                _navigationManager.NavigateTo("organizationList");
            }
            dispatcher.Dispatch(new LoadOrganizationsAction());
        }


        [EffectMethod(typeof(SettingOrganizationLoadStateAction))]
        public async Task LoadState(IDispatcher dispatcher)
        {
            try
            {
                var organizationSettingState = await _localStorageService.GetItemAsync<OrganizationSettingState>(StatePersistenceName);
                if (organizationSettingState is not null)
                {
                    dispatcher.Dispatch(new SetOrganizationStateAction(organizationSettingState));
                    dispatcher.Dispatch(new LoadOrganizationStateSuccessAction());
                }
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new LoadOrganizationStateFailureAction(ex.Message));
            }
        }

        [EffectMethod(typeof(ClearOrganizationStateAction))]
        public async Task ClearState(IDispatcher dispatcher)
        {
            try
            {
                await _localStorageService.RemoveItemAsync(StatePersistenceName);
                dispatcher.Dispatch(new SetOrganizationStateAction(new OrganizationSettingState
                {
                    Initialized = false,
                    Loading = false,
                    OrganizationList = Array.Empty<Organization>(),
                    SelectedOrganization = null,
                    Mode = SettingMode.None

                })); ;
                dispatcher.Dispatch(new ClearOrganizationStateSuccessAction());
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new ClearOrganizationStateFailureAction(ex.Message));
            }
        }
        [EffectMethod(typeof(OrganizationSettingReturnAction))]
        public async Task OrganizationSettingReturn(IDispatcher dispatcher)
        {
            _navigationManager.NavigateTo("organizationList");
        }
    }
    #region Actions
    public record LoadOrganizationsAction();
    public record LoadOrganizationsSuccessAction();

    public record SettingOrganizationLoadStateAction();

    public record SetOrganizationStateAction(OrganizationSettingState State);
    public record LoadOrganizationStateSuccessAction();
    public record LoadOrganizationStateFailureAction(string ErrorMessage);

    public record ClearOrganizationStateAction();
    public record ClearOrganizationStateSuccessAction();
    public record ClearOrganizationStateFailureAction(string ErrorMessage);

    public record OrganizationSettingBeginAction(Organization SelectedData, SettingMode Mode);
    public record OrganizationSettingAction(Organization SelectedData);
    public record OrganizationSettingSuccessAction();
    public record OrganizationSettingFailureAction(string ErrorMessage);

    public record OrganizationSettingReturnAction();
    #endregion
}
