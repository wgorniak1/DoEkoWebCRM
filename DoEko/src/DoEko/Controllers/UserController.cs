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
using DoEko.Controllers.Extensions;
using DoEko.ViewComponents.ViewModels;

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

        [HttpGet]
        public IActionResult List()
        {
            //
            if (HttpContext.Request.IsAjaxRequest())
            {
                var model = _userManager.Users
                    .Select(u => new {

                        u.Id,
                        u.UserName,
                        u.FirstName,
                        u.LastName,
                        u.PhoneNumber,
                        u.Email,
                        u.EmailConfirmed,
                        u.AccessFailedCount,
                        u.LockoutEnabled,
                        u.LockoutEnd,
                        _roles = u.Roles.Select(r => new {
                            r.RoleId,
                            _roleManager.Roles.Single(rm => rm.Id == r.RoleId).Name
                        })
                    }).ToList();

                return Json(new { data = model });
            }
            else
            {
                return View();
            }

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
            ////Temporary model for role list
            //var UserIndexVM = new UserIndexViewModel(_userManager, _roleManager);

            //ViewBag.ReturnUrl = returnUrl;
            //ViewBag.RoleIdList = UserIndexVM.RoleList("");
            if (Request.IsAjaxRequest())
            {
                return ViewComponent("MaintainUser");
            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        public ActionResult Edit(Guid userId)
        {
            if (Request.IsAjaxRequest())
            {
                return ViewComponent("MaintainUser", new { userId = userId });
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            IdentityResult result;

            var user = await _userManager.FindByIdAsync(model.Id.ToString());

            if (user == null)
            {
                ModelState.AddModelError("Id", "Nie znaleziono konta u¿ytkownika");

                return BadRequest(ModelState);
            }

            //
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            if (user.Email != model.Email)
            {
                user.Email = model.Email;
                user.EmailConfirmed = false;
            }

            //
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                AddErrors(result);

                return BadRequest(ModelState);
            }

            var currentRole = (await _userManager.GetRolesAsync(user)).First();
            var role = await _roleManager.FindByIdAsync(model.RoleId.ToString());
            if (!await _userManager.IsInRoleAsync(user,role.Name))
            {
                result = await _userManager.RemoveFromRoleAsync(user, currentRole);
                if (!result.Succeeded)
                {
                    AddErrors(result);
                    return BadRequest(ModelState);
                }
                result = await _userManager.AddToRoleAsync(user, role.Name);
                if (!result.Succeeded)
                {
                    AddErrors(result);
                    return BadRequest(ModelState);
                }
            }

            return Ok();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model, string returnUrl = null)
        {
            ViewBag.ReturnUrl= returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.UserName,
                    Email = model.Email,
                    EmailConfirmed = true,
                    PasswordChangedOn = DateTime.UtcNow
                };

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
                        //return RedirectToAction("Index");

                        return Created("",user.Id);
                    }
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            //return View(model);
            return BadRequest(ModelState);
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