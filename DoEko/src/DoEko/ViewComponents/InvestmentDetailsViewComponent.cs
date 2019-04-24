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
    public class InvestmentDetailsViewComponent : ViewComponent
    {
        private DoEkoContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public InvestmentDetailsViewComponent(DoEkoContext context,UserManager<ApplicationUser> um, RoleManager<IdentityRole> rm )
        {
            _context = context;
            _userManager = um;
            _roleManager = rm;
        }
        public async Task<IViewComponentResult> InvokeAsync(Guid? investmentId, ViewMode viewMode)
        {
            ApplicationUser _user = await _userManager.GetUserAsync(UserClaimsPrincipal);
            _user = await _userManager.Users.Include(u => u.Projects).SingleAsync(u => u.Id == _user.Id);

            var qry = _context.Investments
                .Include(i => i.Address)
                .Include(i => i.Contract);

            var model = viewMode == ViewMode.Display ? 
                await qry.AsNoTracking().SingleAsync(i => i.InvestmentId == investmentId) : 
                await qry.AsTracking().SingleAsync(i => i.InvestmentId == investmentId);

            //ViewData["InvAddrStateId"] = AddressesController.GetStates(_context, model.Address.StateId);
            //ViewData["InvAddrDistrictId"] = AddressesController.GetDistricts(_context, model.Address.StateId, model.Address.DistrictId);
            //ViewData["InvAddrCommuneId"] = AddressesController.GetCommunes(_context, model.Address.StateId, model.Address.DistrictId, model.Address.CommuneId, model.Address.CommuneType);
            //model.Address.CommuneId = model.Address.CommuneId * 10 + (int)model.Address.CommuneType;

            ViewData["ContractId"] = ContractsController.GetOpenContracts(_context, model.ContractId, _user);

            IdentityRole inspectorRole = await _roleManager.FindByNameAsync(Roles.Inspector);

            var users = (await _userManager.GetUsersInRoleAsync(inspectorRole.Name)).OrderBy(u=>u.FullName);
                //.Users
                //.Where(u => u.Roles.Select(r => r.RoleId).Contains(inspectorRole.Id))
                //.OrderBy(k => k.FullName)
                //.ToList();

            ViewData["InspectorId"] = new SelectList(users, "Id", "FullName",model.InspectorId);

            if (User.IsInRole(Roles.Admin))
            {
                if (viewMode == ViewMode.Maintain)
                    return View("EditAdmin", model);
                else
                    return View("DisplayAdmin", model);
            }
            else
            {
                if (viewMode == ViewMode.Maintain)
                    return View("EditOther", model);
                else
                    return View("DisplayOther", model);
            }
        }
    }
}
