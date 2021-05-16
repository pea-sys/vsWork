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
        [StringLength(100, MinimumLength = 3, ErrorMessage = "ユーザIDは3文字以上にしてください。")]
        [Required(ErrorMessage = "ユーザIDを入力してください。")]
        public string Id { get; set; }

        [StringLength(100, MinimumLength = 8, ErrorMessage = "パスワードは8文字以上にしてください。")]
        [Required(ErrorMessage = "パスワードを入力してください。")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Date)]
        public DateTime ActiveDate { get; set; }
    }
}
