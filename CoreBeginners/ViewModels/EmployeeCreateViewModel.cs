using CoreBeginners.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBeginners.ViewModels
{
    public class EmployeeCreateViewModel
    {
        public int Id { get; set; }
        [Required, MaxLength(50, ErrorMessage = "Valid Name")]
        public string Name { get; set; }
        [Display(Name = "Office Email")]
        [Required, RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Valid Email")]
        public string Email { get; set; }
        [Required]
        public Dept? Department { get; set; }
        public List<IFormFile> Photos { get; set; }
        public string ExistingPhotoPath { get; set; }
    }
}
