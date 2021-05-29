using System;


namespace vsWork.Data
{
    public class Session : BaseEntity
    {
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public DateTime CreateTimeStamp { get; set; }
    }
}
