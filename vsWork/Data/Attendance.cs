using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vsWork.Utils;
namespace vsWork.Data
{
    public class Attendance : BaseEntity
    {
        public string UserId { get; set; }
        public int AttendanceCount { get;private set; }
        public DateTime PunchInTimeStamp { get; private set; }
        public DateTime PunchOutTimeStamp { get; private set; }
    }
}
