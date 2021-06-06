using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vsWork.Store.SettingUserUseCase
{
    public static class UserReducers
    {
        [ReducerMethod]
        public static UserState ReduceSetterUserAction(UserState state, UserSelectAction action) =>
          new UserState(settingUser: action.SelectUser);
    }
}
