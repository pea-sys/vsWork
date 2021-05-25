using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vsWork.Data
{
    public class Session : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string SessionId { get; set; }

        public string UserId { get; set; }
        public DateTime CreateTimeStamp{ get; set; }
    }
}
