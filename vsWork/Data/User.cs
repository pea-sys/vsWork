using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vsWork.Data
{
    public class User : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [StringLength(100, MinimumLength = 3)]
        [Required]
        public string id { get; set; }

        [StringLength(100, MinimumLength = 8)]
        [Required]
        public string Password { get; set; }

        [DataType(DataType.Date)]
        public DateTime ActiveDate { get; set; }
    }
}
