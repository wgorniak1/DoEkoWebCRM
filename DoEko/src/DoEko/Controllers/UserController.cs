using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using DoEko.Models.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using DoEko.ViewModels.UserViewModel;

namespace DoEko.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        
        public async Task<IActionResult> Index(UserIndexSortOrder sortOrder, 
                                               string currentFilter, 
                                               string currentRole,
                                               string searchString, 
                                               string RoleId, 
                                               int? page)
        {
            // Initialize
            if (string.IsNullOrEmpty(searchString))
            {
                searchString = currentFilter;
            }
            if (string.IsNullOrEmpty(RoleId))
            {
                RoleId = currentRole;
            }

            //Set Model
            var UserIndexVM = new UserIndexViewModel(_userManager, _roleManager);

            await UserIndexVM.Select(searchString, RoleId);

            UserIndexVM.OrderBy(sortOrder);

            // Set ViewBag
            ViewBag.NameSortParam = sortOrder == UserIndexSortOrder.NameDesc ? "" : UserIndexSortOrder.NameDesc.ToString();
            ViewBag.MailSortParam = sortOrder == UserIndexSortOrder.MailAsc ? UserIndexSortOrder.MailDesc.ToString() : UserIndexSortOrder.MailAsc.ToString();
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort   = sortOrder;
            ViewBag.CurrentRole   = RoleId;
            ViewBag.RoleIdList = UserIndexVM.RoleList(RoleId);
            // Set View
            return View(UserIndexVM);
        }

        [HttpGet]
        public ActionResult Create(string returnUrl = null)
        {
            //Temporary model for role list
            var UserIndexVM = new UserIndexViewModel(_userManager, _roleManager);

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.RoleIdList = UserIndexVM.RoleList("");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model, string returnUrl = null)
        {
            ViewBag.ReturnUrl= returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var name = await _roleManager.GetRoleNameAsync(await _roleManager.FindByIdAsync(model.RoleId));
                    result = await _userManager.AddToRoleAsync(user, name );
                    if (result.Succeeded)
                    {
                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                        // Send an email with this link
                        //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                        //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                        //    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
                        //await _signInManager.SignInAsync(user, isPersistent: false);
                        //_logger.LogInformation(3, "User created a new account with password.");
                        return RedirectToAction("Index");
                    }
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [HttpGet]
        public async Task<ActionResult> Details(string id, string returnUrl = null)
        {
            //Temporary model for role list
            IdentityRole userRole;
            var appUser = await _userManager.FindByIdAsync(id);
            try
            {
                var userRoleNames     = await _userManager.GetRolesAsync(appUser);
                userRole = await _roleManager.FindByNameAsync(userRoleNames.FirstOrDefault());
            }
            catch (System.Exception)
            {
                userRole = new IdentityRole();
            }

            var UserDetailsVM = new UserDetailsViewModel
            {
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                UserID = appUser.Id,
                UserName = appUser.UserName,
                Email = appUser.Email,
                RoleId = userRole.Id
            };
            //
            ViewData["RoleId"] = new SelectList(_roleManager.Roles.ToList(), "Id", "Name", userRole);
            
            return View(UserDetailsVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(UserDetailsViewModel model, string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserID);
                user.LastName = model.LastName;
                user.FirstName = model.FirstName;
                //var result = await _userManager.SetEmailAsync(user, model.Email);
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {

                    var userRoleNames = await _userManager.GetRolesAsync(user);
                    IdentityRole userRole = await _roleManager.FindByNameAsync(userRoleNames.FirstOrDefault());
                    if (userRole.Id != model.RoleId)
                    {
                        var role = _roleManager.Roles.Where(r => r.Id == model.RoleId).First();
                        result = await _userManager.AddToRoleAsync(user, role.Name);
                    }

                    if (result.Succeeded)
                    {
                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                        // Send an email with this link
                        //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                        //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                        //    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
                        //await _signInManager.SignInAsync(user, isPersistent: false);
                        //_logger.LogInformation(3, "User created a new account with password.");
                        return RedirectToAction("Index");
                    }
                }
                AddErrors(result);
            }
            
            ViewData["RoleId"] = new SelectList(_roleManager.Roles.ToList(), "Id", "Name", model.RoleId);

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string Id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(Id);
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                
                    return Ok();
                else
                    return Json("Error");
                
            }
            catch (Exception)
            {
                return Json(new { result = false, responsemessage = "B³¹d" });
            }
        }



        #region Helpers
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
        #endregion
    }
}