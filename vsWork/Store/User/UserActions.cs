using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using vsWork.Data;
namespace vsWork.Store.SettingUserUseCase
{
    public record UserSelectAction(User SelectUser);
    public record UserUpdateAction(User UpdateUser);
    public record UserUpdateSuccessAction();
}
