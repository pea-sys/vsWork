using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vsWork.Utils;
namespace vsWork.Data
{
    public class UserState : BaseEntity
    {
        public string UserId { get; set; }
        public StateType State { get; set; }
        public DateTime TimeStamp { get; set; }

        public enum StateType
        {
            [EnumText("")]
            None = 0,
            [EnumText("勤務中")]
            PunchIn = 1,
            [EnumText("退勤済")]
            PunchOut = 2
        }
    }
}
