using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoreBeginners.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }
        // GET: /<controller>/
        [Route("Error/{statuscode}")]
        public IActionResult HttpStatusCodeHandler(int statuscode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            switch (statuscode){
                case 404:
                    ViewBag.ErrorMessage = "Sorry, The page could not be found";
                    logger.LogWarning($"404 Error Occured. Path{statusCodeResult.OriginalPath} and QueryString={statusCodeResult.OriginalQueryString}");
                    //ViewBag.Path = statusCodeResult.OriginalPath;
                    //ViewBag.QS = statusCodeResult.OriginalQueryString;
                    break;
            }
            return View("NotFound");
        }
        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var ExceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            //ViewBag.ExceptionPath = ExceptionDetails.Path;
            //ViewBag.Message = ExceptionDetails.Error.Message;
            //ViewBag.Trace = ExceptionDetails.Error.StackTrace;

            logger.LogError($"The Path {ExceptionDetails.Path} threw an Exception {ExceptionDetails.Error}");
            return View("Error");
        }
    }
}
