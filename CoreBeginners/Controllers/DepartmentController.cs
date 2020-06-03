using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CoreBeginners.Controllers
{
    public class DepartmentController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}
        public string List()
        {
            return "List () of Department Controller";
        }
        public string Details()
        {
            return "Details () of Department Controller";
        }
    }
}