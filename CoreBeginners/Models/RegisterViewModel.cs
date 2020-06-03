using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CoreBeginners.Models;

namespace CoreBeginners.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Remote(action: "CheckEmailFound", controller:"Account")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Password and Confirm Password do not match.")]
        [Display(Name ="Confirm Password")]
        public string  ConfirmPassword { get; set; }
        [Required]
        public string City { get; set; }
    }
}
