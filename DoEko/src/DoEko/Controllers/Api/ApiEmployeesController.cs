using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoEko.Models.DoEko;
using DoEko.Models.Payroll;
using Microsoft.AspNetCore.Authorization;
using DoEko.Models.Identity;
using DoEko.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;

namespace DoEko.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/v1/Employees")]
    [Authorize()]
    public class ApiEmployeesController : Controller
    {
        private readonly DoEkoContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApiEmployeesController(DoEkoContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: api/ApiEmployees
        [HttpPost]
        [Route("CreateFromUser")]
        public async Task<IActionResult> CreateEmployeeFromUser([FromBody] Guid UserId)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            try
            {
                var user = await _userManager.FindByIdAsync(UserId.ToString());
                var ee = Employee.CreateFromUser(user);

                _context.Employees.Add(ee);
                int result = await _context.SaveChangesAsync();

                return CreatedAtAction("CreateEmployee", new { id = ee.EmployeeId }, ee);
            }
            catch (Exception exc)
            {
                ModelState.Clear();
                ModelState.AddModelError("", exc.Message);
                return BadRequest(ModelState);
            }

        }


        // POST: api/ApiEmployees
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            try
            {
                _context.Employees.Add(employee);
                int result = await _context.SaveChangesAsync();

            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
                return BadRequest(ModelState);
            }

            return CreatedAtAction("CreateEmployee", new { id = employee.EmployeeId }, employee);
        }


        [HttpPost]
        [Route("{employeeId:Guid}/Rates")]
        public async Task<IActionResult> InsertNewRate([FromRoute] Guid employeeId, [FromBody] EmployeeBasicPay basicPay)
        {
            //post means insert new user

            basicPay.EmployeeId = employeeId;
            basicPay.End = DateTime.MaxValue;

            if (!basicPay.ContractId.HasValue)
            {
                var current = _context.EmployeesBasicPay.Where(eu => eu.EmployeeId == employeeId &&
                                                                     eu.End == DateTime.MaxValue &&
                                                                     eu.ContractId.HasValue == false).FirstOrDefault();
                if (current != null)
                {
                    current.End = new DateTime(basicPay.Start.Year, basicPay.Start.Month, basicPay.Start.Day, 23, 59, 59).AddDays(-1);
                    _context.EmployeesBasicPay.Update(current);
                };
            }
            else
            {
                var current = _context.EmployeesBasicPay.Where(eu => eu.EmployeeId == employeeId &&
                                                                     eu.End == DateTime.MaxValue &&
                                                                     eu.ContractId.HasValue == true &&
                                                                     eu.ContractId.Value == basicPay.ContractId).FirstOrDefault();
                if (current != null)
                {
                    current.End = new DateTime(basicPay.Start.Year, basicPay.Start.Month, basicPay.Start.Day, 23, 59, 59).AddDays(-1);
                    _context.EmployeesBasicPay.Update(current);
                };
            }

            _context.Add(basicPay);

            try
            {
                int result = await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception exc)
            {
                ModelState.Clear();
                ModelState.AddModelError("", exc.Message);
                return BadRequest(ModelState);
            }

        }


        [HttpPost]
        [Route("{employeeId:Guid}/Users")]
        public async Task<IActionResult> InsertNewUser([FromRoute] Guid employeeId, [FromBody] EmployeeUser EmployeeUser)
        {
            //post means insert new user

            EmployeeUser.EmployeeId = employeeId;
            EmployeeUser.End = DateTime.MaxValue;

            var current = _context.EmployeesUsers.Where(eu => eu.EmployeeId == employeeId && 
                                                            eu.End == DateTime.MaxValue).FirstOrDefault();
            if (current != null)
            {
                current.End = new DateTime(EmployeeUser.Start.Year, EmployeeUser.Start.Month, EmployeeUser.Start.Day, 23, 59, 59).AddDays(-1);
                _context.EmployeesUsers.Update(current);
            };

            _context.Add(EmployeeUser);

            try
            {
                int result = await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception exc)
            {
                ModelState.Clear();
                ModelState.AddModelError("", exc.Message);
                return BadRequest(ModelState);
            }

        }
        // GET: api/ApiEmployees
        [HttpGet]
        [Route("List")]
        public async Task<IEnumerable<EmployeeVM>> GetEmployees()
        {
            var list = await _context.Employees
                .Include(e => e.Address).ThenInclude(a=>a.State)
                .Include(e => e.Address).ThenInclude(a=>a.District)
                .Include(e => e.Address).ThenInclude(a=>a.Commune)
                .Include(e => e.BasicPay)
                .Include(e => e.Users)
                .ToListAsync();

            var result = new Collection<EmployeeVM>();

            foreach (var e in list)
            {
                var itemvm = new EmployeeVM
                {
                    Address = e.Address,
                    AddressId = e.AddressId,
                    BasicPay = e.BasicPay,
                    BusinessPartnerId = e.BusinessPartnerId,
                    Email = e.Email,
                    FirstName = e.FirstName,
                    IdNumber = e.IdNumber,
                    LastName = e.LastName,
                    Pesel = e.Pesel,
                    PhoneNumber = e.PhoneNumber,
                    TaxId = e.TaxId,
                    Users = new Collection<EmployeeUserVM>()
                };

                foreach (var u in e.Users)
                {
                    itemvm.Users.Add(new EmployeeUserVM
                    {
                        EmployeeId = u.EmployeeId,
                        End = u.End,
                        Start = u.Start,
                        UserId = u.UserId,
                        User = await _userManager.FindByIdAsync(u.UserId.ToString())
                    });
                }
    
                itemvm.CurrentUser = await _userManager.FindByIdAsync(e.CurrentUserId.ToString());
                result.Add(itemvm);

            }
            
            return result.ToList();
        }

        // GET: api/ApiEmployees/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = await _context.Employees.SingleOrDefaultAsync(m => m.BusinessPartnerId == id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // PUT: api/ApiEmployees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee([FromRoute] Guid id, [FromBody] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != employee.BusinessPartnerId)
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // DELETE: api/ApiEmployees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = await _context.Employees.SingleOrDefaultAsync(m => m.BusinessPartnerId == id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return Ok(employee);
        }

        private bool EmployeeExists(Guid id)
        {
            return _context.Employees.Any(e => e.BusinessPartnerId == id);
        }
    }
}