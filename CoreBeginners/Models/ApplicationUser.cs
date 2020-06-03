using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBeginners.Models
{
    //This class is created for more property added in IdentityUser class
    //Employee is another class but this another record store 
    public class ApplicationUser :IdentityUser
    {
        public string City { get; set; }
    }
}
