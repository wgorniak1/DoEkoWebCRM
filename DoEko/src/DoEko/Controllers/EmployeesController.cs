using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DoEko.Models.Payroll;
using Microsoft.AspNetCore.Identity;
using DoEko.Models.DoEko;
using DoEko.Models.Identity;

namespace DoEko.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DoEkoContext _context;

        public EmployeesController(DoEkoContext doEkoContext, UserManager<ApplicationUser> userManager)
        {
            _context = doEkoContext;
            _userManager = userManager;
        }
        public IActionResult Index() => View();


        public async Task<IActionResult> Create(Guid? fromUserId)
        {
            return fromUserId.HasValue ? 
                PartialView("_CreateEmployee", new Employee()) :
                PartialView("_CreateEmployee", Employee.CreateFromUser(await _userManager.FindByIdAsync(fromUserId.ToString())));
        }
    }


}