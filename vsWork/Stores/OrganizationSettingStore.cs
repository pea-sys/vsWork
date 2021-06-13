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
    public class OrganizationSettingStore
    {
        public class OrganizationSettingFeature : Feature<SettingState<Organization>>
        {
            public override string GetName() => "OrganizationSetting";

            protected override SettingState<Organization> GetInitialState()
            {
                return new SettingState<Organization>
                {
                    Initialized = false,
                    Loading = false,
                    ListData = Array.Empty<Organization>(),
                    SelectedData = null,
                    Mode = SettingMode.None
                };
            }
        }
        public static class OrganizationSettingReducers
        {
            [ReducerMethod]
            public static SettingState<Organization> OnSetOrganizations(SettingState<Organization> state, SetOrganizationsAction action)
            {
                return state with
                {
                    ListData = (Organization[])(action.ListData),
                    Loading = false,
                    Initialized = true,
                    SelectedData = null
                };
            }

            [ReducerMethod(typeof(LoadOrganizationsAction))]
            public static SettingState<IEntity> OnLoadLoadList(SettingState<IEntity> state)
            {
                return state with
                {
                    Loading = true,
                    Mode = SettingMode.None
                };
            }

            [ReducerMethod]
            public static SettingState<Organization> OnOrganizationSettingSetState(SettingState<Organization> state, SetOrganizationStateAction action)
            {
                return action.State;
            }
            [ReducerMethod]
            public static SettingState<Organization> OnSettingBegin(SettingState<Organization> state, OrganizationSettingBeginAction action)
            {
                return state with
                {
                    SelectedData = action.SelectedData,
                    Mode = action.Mode
                };
            }
            [ReducerMethod]
            public static SettingState<IEntity> OnSettingOrganization(SettingState<IEntity> state, OrganizationSettingAction action)
            {
                return state with
                {
                    SelectedData = action.SelectedData
                };
            }
            [ReducerMethod]
            public static SettingState<IEntity> OnSettingOrganizationSuccess(SettingState<IEntity> state, OrganizationSettingSuccessAction action)
            {
                return state with
                {
                    SelectedData = null,
                };
            }
            [ReducerMethod]
            public static SettingState<IEntity> OnSettingOrganizationFailure(SettingState<IEntity> state, OrganizationSettingFailureAction action)
            {
                return state with
                {
                    // 未定
                };
            }

        }
    }
    public class OrganizationSettingEffects
    {

        private readonly IState<SettingState<Organization>> SettingState;
        private readonly IRepository<Organization, string> _organizationRepositoryService;
        private readonly ILocalStorageService _localStorageService;
        private readonly NavigationManager _navigationManager;
        private const string StatePersistenceName = "OrganizationSettingState";

        public OrganizationSettingEffects
        (IState<SettingState<Organization>> settingState,
        IRepository<Organization, string> organizationRepositoryService,
        ILocalStorageService localStorageService,
        NavigationManager navigationManager)
        {
            SettingState = settingState;
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
                dispatcher.Dispatch(new OrganizationSettingAction(SettingState.Value.SelectedData));
            }

        }
        [EffectMethod(typeof(OrganizationSettingAction))]
        public async Task Setting(IDispatcher dispatcher)
        {
            try
            {
                if (SettingState.Value.Mode == SettingMode.Add)
                {
                    _organizationRepositoryService.Add(SettingState.Value.SelectedData);
                    dispatcher.Dispatch(new OrganizationSettingSuccessAction());
                }
                else if (SettingState.Value.Mode == SettingMode.Update)
                {
                    _organizationRepositoryService.Update(SettingState.Value.SelectedData);
                    dispatcher.Dispatch(new OrganizationSettingSuccessAction());
                }
                else if (SettingState.Value.Mode == SettingMode.Delete)
                {
                    _organizationRepositoryService.Remove(SettingState.Value.SelectedData.OrganizationId);
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
                var organizationSettingState = await _localStorageService.GetItemAsync<SettingState<Organization>>(StatePersistenceName);
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
                dispatcher.Dispatch(new SetOrganizationStateAction(new SettingState<Organization>
                {
                    Initialized = false,
                    Loading = false,
                    ListData = Array.Empty<Organization>(),
                    SelectedData = null,
                    Mode = SettingMode.None

                })); ;
                dispatcher.Dispatch(new ClearOrganizationStateSuccessAction());
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new ClearOrganizationStateFailureAction(ex.Message));
            }
        }
    }
    #region Actions
    public record LoadOrganizationsAction();
    public record LoadOrganizationsSuccessAction();
    public record SetOrganizationsAction(Organization[] ListData);

    public record SettingOrganizationLoadStateAction();

    public record SetOrganizationStateAction(SettingState<Organization> State);
    public record LoadOrganizationStateSuccessAction();
    public record LoadOrganizationStateFailureAction(string ErrorMessage);

    public record ClearOrganizationStateAction();
    public record ClearOrganizationStateSuccessAction();
    public record ClearOrganizationStateFailureAction(string ErrorMessage);

    public record OrganizationSettingBeginAction(Organization SelectedData, SettingMode Mode);
    public record OrganizationSettingAction(Organization SelectedData);
    public record OrganizationSettingSuccessAction();
    public record OrganizationSettingFailureAction(string ErrorMessage);
    #endregion
}
