using CoreBeginners.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBeginners.ViewModels
{
    public class HomeControllerViewModel
    {
        public Employee Employee { get; set; }
        public string  PageTitle { get; set; }
    }
}
