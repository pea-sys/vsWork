using Blazored.LocalStorage;
using Fluxor;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vsWork.Data;

namespace vsWork.Stores
{
    public record HolidaySettingState : BaseSettingState
    {
        public Holiday[] HolidayList { get; init; }
        public Holiday SelectedHoliday { get; set; }
    }
    public class HolidaySettingStore
    {
        public class HolidaySettingFeature : Feature<HolidaySettingState>
        {
            public override string GetName() => "HolidaySetting";

            protected override HolidaySettingState GetInitialState()
            {
                return new HolidaySettingState
                {
                    Initialized = false,
                    Loading = false,
                    HolidayList = Array.Empty<Holiday>(),
                    SelectedHoliday = null,
                    Mode = SettingMode.None,
                };
            }
        }
        [ReducerMethod]
        public static HolidaySettingState OnUserSettingSetState(HolidaySettingState state, SetHolidayStateAction action)
        {
            return action.State;
        }
        public static class HolidaySettingReducers
        {
            [ReducerMethod]
            public static HolidaySettingState OnSetHolidays(HolidaySettingState state, SetHolidaysAction action)
            {
                return state with
                {
                    HolidayList = (Holiday[])(action.ListData),
                    Loading = false,
                    Initialized = true,
                    SelectedHoliday = null
                };
            }
            [ReducerMethod(typeof(LoadHolidaysAction))]
            public static HolidaySettingState OnLoadList(HolidaySettingState state)
            {
                return state with
                {
                    Loading = true,
                    Mode = SettingMode.None,
                    SelectedHoliday = null
                };
            }
            [ReducerMethod]
            public static HolidaySettingState OnSettingBegin(HolidaySettingState state, HolidaySettingBeginAction action)
            {
                return state with
                {
                    SelectedHoliday = action.SelectedData,
                    Mode = action.Mode
                };
            }
            [ReducerMethod]
            public static HolidaySettingState OnSettingUser(HolidaySettingState state, HolidaySettingAction action)
            {
                return state with
                {
                    SelectedHoliday = action.SelectedData
                };
            }
        }
        public class HolidaySettingEffects
        {
            private readonly IState<HolidaySettingState> SettingState;
            private readonly IRepository<Holiday, (int, DateTime)> _holidayRepositoryService;
            private readonly IState<CurrentUserState> _currentUserState;
            private readonly ILocalStorageService _localStorageService;
            private readonly NavigationManager _navigationManager;
            private const string StatePersistenceName = "HolidaySettingState";

            public HolidaySettingEffects
            (IState<HolidaySettingState> settingState,
            IRepository<Holiday, (int, DateTime)> holidayRepositoryService,
            IState<CurrentUserState> currentUserState,
            ILocalStorageService localStorageService,
            NavigationManager navigationManager)
            {
                SettingState = settingState;
                _holidayRepositoryService = holidayRepositoryService;
                _currentUserState = currentUserState;
                _localStorageService = localStorageService;
                _navigationManager = navigationManager;
            }

            [EffectMethod(typeof(ClearHolidayStateAction))]
            public async Task ClearState(IDispatcher dispatcher)
            {
                try
                {
                    await _localStorageService.RemoveItemAsync(StatePersistenceName);
                    dispatcher.Dispatch(new SetHolidayStateAction(new HolidaySettingState
                    {
                        Initialized = false,
                        Loading = false,
                        HolidayList = Array.Empty<Holiday>(),
                        SelectedHoliday = null,
                        Mode = SettingMode.None,
                    })); ;
                    dispatcher.Dispatch(new ClearUserStateSuccessAction());
                }
                catch (Exception ex)
                {
                    dispatcher.Dispatch(new ClearUserStateFailureAction(ex.Message));
                }
            }
        }
    }
    public record LoadHolidaysAction();
    public record LoadHolidaysSuccessAction();
    public record LoadHolidaysFailureAction(string ErrorMessage);

    public record SetHolidaysAction(Holiday[] ListData);

    public record ClearHolidayStateAction();
    public record ClearHolidayStateSuccessAction();
    public record ClearHolidayStateFailureAction(string ErrorMessage);

    public record SetHolidayStateAction(HolidaySettingState State);

    public record HolidaySettingBeginAction(Holiday SelectedData, SettingMode Mode);
    public record HolidaySettingAction(Holiday SelectedData);
    public record HolidaySettingSuccessAction();
    public record HolidaySettingFailureAction(string ErrorMessage);
}
