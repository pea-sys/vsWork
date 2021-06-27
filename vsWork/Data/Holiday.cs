using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vsWork.Utils;
namespace vsWork.Data
{
    public class Holiday : BaseEntity
    {
        public int OrganizationId { get; set; }
        public DateTime Date { get; set; }
        public HolidayType HolidayType { get; set; }
        public string Name { get; set; }
        public string DateString
        {
            get
            {
                return Date.ToLongDateString();
            }
        }
    }
    public enum HolidayType
    {
        None,
        [EnumText("日付固定")]
        FixedDay,
        [EnumText("曜日固定")]
        FixedWeekofDay,
        [EnumText("移動祝日")]
        Moveable,
    }
}
