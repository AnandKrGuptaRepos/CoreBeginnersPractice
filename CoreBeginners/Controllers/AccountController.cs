using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreBeginners.Models;
using CoreBeginners.ViewModels;
using DNTCaptcha.Core;
using DNTCaptcha.Core.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoreBeginners.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<AccountController> logger;
        private readonly IDNTCaptchaValidatorService dNTCaptcha;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger, IDNTCaptchaValidatorService dNTCaptcha)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.dNTCaptcha = dNTCaptcha;
        }
        [AcceptVerbs("Get","Post")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckEmailFound(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user==null)
            {
               return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use.");
            }
        }
       

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    City=model.City
                };
                var result =await userManager.CreateAsync(user,model.Password);
                if (result.Succeeded)
                {
                    var tokens = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var ConfirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = tokens }, Request.Scheme);
                    logger.Log(LogLevel.Warning, ConfirmationLink);
                    if (signInManager.IsSignedIn(User)&& User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }
                  // await signInManager.SignInAsync(user, isPersistent: false);
                   // return RedirectToAction("Index", "Home");
                }
               
                //foreach(var error in result.Errors)
                //{
                //    ModelState.AddModelError("", error.Description);
                //}
                // Temporary Basis only
                ViewBag.ErrorTitle = "Registration Successful";
                ViewBag.ErrorMessage = "Before you can login,Please Confirm your Email by click on the Confirmation link we have emailed to you";
                return View("Error");
            }
            

            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userid, string tokens)
        {
            if(userid== null || tokens == null)
            {
               return RedirectToAction("index", "home");
            }
            var user= await userManager.FindByIdAsync(userid);
            if(user== null)
            {
                ViewBag.ErrorMessage = $"The user id ={userid} is invalid";
            }
            var result = await userManager.ConfirmEmailAsync(user, tokens);
            if (result.Succeeded)
            {
                return View();
            }
            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("Error");
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
           await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        [AllowAnonymous]
      
         public async Task<IActionResult> Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl=returnUrl,
                ExternalLogins=(await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        //[ValidateDNTCaptcha(
        //    ErrorMessage ="Please Enter Valid Captch",CaptchaGeneratorLanguage =Language.English,CaptchaGeneratorDisplayMode =DisplayMode.SumOfTwoNumbers)]
        public async Task<IActionResult> Login(LoginViewModel model,string ReturnUrl)
        {
           model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                // Start of Email Confirmation Code
                var user = await userManager.FindByEmailAsync(model.Email);
                if(user!=null && !user.EmailConfirmed && (await userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError("","Email not Confirmed yet");
                    return View(model);
                }
                // End of Email Confirmation Code
                var result =await signInManager.PasswordSignInAsync(model.Email,model.Password,model.RememberMe,true);
                if (!dNTCaptcha.HasRequestValidCaptchaEntry(Language.English, DisplayMode.ShowDigits))
                {
                    this.ModelState.AddModelError(DNTCaptchaTagHelper.CaptchaInputName, "Please Enter Valid Captcha.");
                    return View(model);
                }
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(ReturnUrl)&& Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                   
                }
                if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider,string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
             return new ChallengeResult(provider, properties); 
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl=null, string remoteError=null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            LoginViewModel loginViewModel = new LoginViewModel
            {
                ReturnUrl=returnUrl,
                ExternalLogins=(await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            if (remoteError != null)
            {
                ModelState.AddModelError("",$"Error from External Provider {remoteError}");
                return View("Login", loginViewModel);
            }
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError("", $"Error loading external login information");
                return View("Login", loginViewModel);
            }
             var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser user = null;
            if(email!=null)
            {
                user = await userManager.FindByEmailAsync(email);
                if(user!=null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError("", "Email not Confirmed yet1");
                    return View("Login",loginViewModel);
                }
            }
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
                                                                       isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                //var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {
                  //  var user = await userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName=info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email=info.Principal.FindFirstValue(ClaimTypes.Email)
                        };
                        await userManager.CreateAsync(user);
                    }
                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                ViewBag.ErrorTitle = $"Email Claim not received from : {info.LoginProvider}";
                ViewBag.ErrorMessage = $"Please contact support on Anand@gupta.com";
                return View("Error");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    var tokens = await userManager.GeneratePasswordResetTokenAsync(user);
                    var PasswordResetLink = Url.Action("ResetPassword", "Account", new { Email = model.Email, token = tokens }, Request.Scheme);
                    logger.Log(LogLevel.Warning, PasswordResetLink);
                    return View("ForgotPasswordConfirmation");
                }
                return View("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string Email)
        {
            if(token==null && Email == null)
            {
                ModelState.AddModelError("","Invalid Password reset token");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if(user!=null)
                {
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        if(await userManager.IsLockedOutAsync(user))
                        {
                            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                        }
                        return View("ResetPasswordConfirmation");
                    }
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
                return View("ResetPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await userManager.GetUserAsync(User);
            var userHasPassword = await userManager.HasPasswordAsync(user);
            if (!userHasPassword)
            {
                return RedirectToAction("AddPassword");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }
                var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.ConfirmPassword);
                if (!result.Succeeded)
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
                //////This line to refresh the logged in user in sign in Cookie
                await signInManager.RefreshSignInAsync(user);
                return View("ResetPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddPassword()
        {
            var user = await userManager.GetUserAsync(User);
            var userHaspassword = await userManager.HasPasswordAsync(user);
            if(userHaspassword)
            {
                return RedirectToAction("ChangePassword");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPassword(AddLocalPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                var result = await userManager.AddPasswordAsync(user,model.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View();
                }
                await signInManager.RefreshSignInAsync(user);
                return View("AddPasswordConfirmation");
            }
            return View();
        }
    }
}
