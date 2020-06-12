using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBeginners.ViewModels
{
    public class AddLocalPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "NewPassword and Confirm Password must Match")]
        [Display(Name = "Confirm New Password")]
        public string ConfirmPassword { get; set; }
    }
}
