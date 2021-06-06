using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vsWork.Data;
namespace vsWork.Store.SettingUserUseCase
{
    public class UserState:IState
    {
        public User SettingUser { get; }
        public UserState(User settingUser)
        {
            SettingUser = Utility.DeepCopy(settingUser);
        }
    }
}
