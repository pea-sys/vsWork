using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vsWork.Data;

namespace vsWork.Store.SettingUserUseCase
{
    public class UserFeature : Feature<UserState>
    {
        public override string GetName() => "SettingUserState";
        protected override UserState GetInitialState() =>
          new UserState(settingUser: new User());
    }
}
