using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreBeginners.Models;
using CoreBeginners.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoreBeginners.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger logger;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager, ILogger<AdministrationController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                var role = await roleManager.CreateAsync(identityRole);
                if (role.Succeeded)
                {
                    return RedirectToAction("ListRole", "Administration");
                }
                foreach (var error in role.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ListRole()
        {
            var result = roleManager.Roles;
            return View(result);
        }

        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(string Id)
        {
            var role = await roleManager.FindByIdAsync(Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id={Id} cannot be found";
                return View("NotFound");
            }
            var result = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };
            foreach (var user in userManager.Users.ToList())
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    result.Users.Add(user.UserName);
                }
            }
            return View(result);
        }

        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id={model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    RedirectToAction("ListRole");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> EditUserInRole(string Id)
        {
            ViewBag.RoleId = Id;
            var role = await roleManager.FindByIdAsync(Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id={Id } can not be found";
                return View("NotFound");
            }
            // var userstemp = userManager.Users;
            var model = new List<UserRoleViewModel>();
            foreach (var user in userManager.Users.ToList())
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }
                model.Add(userRoleViewModel);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserInRole(List<UserRoleViewModel> model, string roleid)
        {
            var role = await roleManager.FindByIdAsync(roleid);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id={roleid } can not be found";
                return View("NotFound");
            }
            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;
                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditRole", new { Id = roleid });
                    }
                }

            }
            return RedirectToAction("EditRole", new { Id = roleid });
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User id={id} is not found";
                return View("NotFound");
            }
            var userClaims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);
            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                City = user.City,
                Email = user.Email,
                Claims = userClaims.Select(c => c.Type+ " : " +c.Value).ToList(),
                Roles = userRoles.ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User id={model.Id} is not found";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.City = model.City;
                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string Id)
        {

            var user = await userManager.FindByIdAsync(Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User id={Id} is not found";
                return View("NotFound");
            }
            else
            {
                try
                {
                    var result = await userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListUsers");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                // return View("ListUsers");
                catch (DbUpdateException ex)
                {
                    logger.LogError($"Exception Occurs : {ex}");
                    ViewBag.ErrorTitle = $"{user.UserName} user is in Use";
                    ViewBag.ErrorMsg = $"{user.UserName} user can not be deleted as there are users in this role. If you want to delete this user, Please remove the users from the user and then try to delete";
                    return View("Error");
                }
            }
            return View("ListUsers");
        }

        [HttpPost]
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteRole(string Id)
        {
            var role = await roleManager.FindByIdAsync(Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"User id={Id} is not found";
                return View("NotFound");
            }
            else
            {
                try
                {
                    var result = await roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRole");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    //  return View("ListRole");

                }
                catch (DbUpdateException ex)
                {
                    logger.LogError($"Exception Occurs : {ex}");
                    ViewBag.ErrorTitle = $"{role.Name} role is in Use";
                    ViewBag.ErrorMsg = $"{role.Name} role can not be deleted as there are users in this role. If you want to delete this role, Please remove the users from the user and then try to delete";
                    return View("Error");
                }
            }
            return View("ListRole");
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRole(string userid)
        {
            ViewBag.UserId = userid;
            var user = await userManager.FindByIdAsync(userid);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User id={userid} is not found";
                return View("NotFound");
            }
            var model = new List<UserRoleViewModel>();
            foreach (var role in roleManager.Roles.ToList())
            {
                var userroleviewmodal = new UserRoleViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userroleviewmodal.IsSelected = true;
                }
                else
                {
                    userroleviewmodal.IsSelected = false;
                }
                model.Add(userroleviewmodal);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRole(List<UserRoleViewModel> model, string userid)
        {
            var user = await userManager.FindByIdAsync(userid);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User id={userid} is not found";
                return View("NotFound");
            }
            var Roles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, Roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Can not remove user existing Roles");
                return View(model);
            }
            result = await userManager.AddToRolesAsync(user, model.Where(x => x.IsSelected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Can not Add selected  roles to users");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = userid });
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserClaim(string userid)
        {
            var user = await userManager.FindByIdAsync(userid);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User id={userid} is not found";
                return View("NotFound");
            }
            var ExistingUserClaim = await userManager.GetClaimsAsync(user);
            var model = new UserClaimViewModel
            {
                UserId = userid
            };
            foreach (Claim claim in ClaimStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };
                if (ExistingUserClaim.Any(c => c.Type.Contains(claim.Type) && c.Value=="true"))
                {
                    userClaim.IsSelected = true;
                }
                model.Claims.Add(userClaim);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserClaim(UserClaimViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User id={model.UserId} is not found";
                return View("NotFound");
            }
            var claim = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimsAsync(user,claim);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Can not remove user's Existing Claims");
                return View(model);
            }
            //result = await userManager.AddClaimsAsync(user, model.Claims.Where(c => c.IsSelected)
            //    .Select(d => new Claim(d.ClaimType,d.ClaimType)));
            ////Boolean type access claim
            result = await userManager.AddClaimsAsync(user, model.Claims.Select(d => new Claim(d.ClaimType, d.IsSelected?"true":"false")));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Can not add selected Claims to user");
                return View(model);
            }
            return RedirectToAction("EditUser",new { Id=model.UserId});
        }
    }
}