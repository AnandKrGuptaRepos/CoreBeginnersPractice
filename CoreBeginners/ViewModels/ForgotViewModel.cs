using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBeginners.ViewModels
{
    public class ForgotViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
