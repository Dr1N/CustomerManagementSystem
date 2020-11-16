using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace App.Models
{
    public class CustomerViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 4)]
        [RegularExpression("^[a-zA-Z0-9._]+$", ErrorMessage = "Invalid login format")]
        public string Login { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Remote(action: "VerifyPassword", controller: "Customer")]
        public string Password { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; }

        [Phone]
        [StringLength(50)]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Roles")]
        public List<string> RoleNames { get; set; }

        [Required]
        public bool Active { get; set; }

        public string Created { get; set; }

        public string CreatedByName { get; set; }

        public string Update { get; set; }

        public string UpdateByName { get; set; }
    }
}
