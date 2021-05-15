using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vsWork.Data
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [StringLength(100, MinimumLength = 3)]
        [Required(ErrorMessage ="Please Input UserID")]
        public string UserID { get; set; }

        [StringLength(100, MinimumLength = 8)]
        [Required(ErrorMessage = "Please Input Password")]
        public string Password { get; set; }

        [DataType(DataType.Date)]
        public DateTime ActiveDate { get; set; }
    }
}
