using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class LoginViewModel
    {
        [Required]
        [StringLength(15)]
        public string Login { get; set; }

        [Required]
        [StringLength(50)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
