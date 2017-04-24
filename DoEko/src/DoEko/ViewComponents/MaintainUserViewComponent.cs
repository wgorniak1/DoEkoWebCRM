using DoEko.Controllers;
using DoEko.Controllers.Helpers;
using DoEko.Models.DoEko;
using DoEko.Models.Identity;
using DoEko.ViewComponents.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.ViewComponents
{
    [ViewComponent]
    public class MaintainUserViewComponent : ViewComponent
    {
        //private readonly DoEkoContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
            
        public MaintainUserViewComponent(//DoEkoContext context, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            //_context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IViewComponentResult> InvokeAsync(Guid? userId, bool editMode = true)
        {

            if (!userId.HasValue || userId.Equals(Guid.Empty) )
            {
                CreateUserViewModel model = new CreateUserViewModel();
                
                ViewData["RoleList"] = new SelectList(_roleManager.Roles.ToList(), "Id", "Name", null);
                ViewData["EditMode"] = editMode;

                return View("CreateUser", model);
            }
            else
            {
                _roleManager.Roles.Load();

                EditUserViewModel model = new EditUserViewModel();
                var appUser = await _userManager.FindByIdAsync(userId.ToString());
                if (appUser != null)
                {
                    model.Email = appUser.Email;
                    model.FirstName = appUser.FirstName;
                    model.Id = Guid.Parse(appUser.Id);
                    model.LastName = appUser.LastName;

                    var roles = await _userManager.GetRolesAsync(appUser);

                    model.RoleId = Guid.Parse((await _roleManager.FindByNameAsync(roles.First())).Id);
                    model.UserName = appUser.UserName;

                    ViewData["RoleList"] = new SelectList(_roleManager.Roles.ToList(), "Id", "Name", _roleManager.FindByNameAsync(roles.First()));
                    ViewData["EditMode"] = editMode;
                }
                                    
                return View("MaintainUser",model);
            }
            

        }
    }
}
