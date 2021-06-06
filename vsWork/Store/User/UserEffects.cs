using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vsWork.Data;
using Fluxor;

namespace vsWork.Store.SettingUserUseCase
{
    public class UserEffects
    {
        private readonly IRepository<User,string> _userRepositoryService;
        private const string SettingUserStatePersistenceName = "vsWork_SettingUserState";

        public UserEffects(IRepository<User,string> userRepositoryService)
        {
            _userRepositoryService = userRepositoryService;
        }

        [EffectMethod]
        public async Task Update(UserUpdateAction action, IDispatcher dispatcher)
        {

           _userRepositoryService.Update(action.UpdateUser);
                dispatcher.Dispatch(new UserUpdateSuccessAction());

        }

        
    }
}
