using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DoEko.Models.DoEko;
using System.Collections.ObjectModel;
using DoEko.Models.Payroll;
using Microsoft.EntityFrameworkCore;

namespace DoEko.Controllers
{
    public class PayrollController : Controller
    {
        private readonly DoEkoContext _context;

        public PayrollController(DoEkoContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Start(DateTime Period)
        {
            var periodIn = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var periodFrom = new DateTime(Period.Year, Period.Month, 1);
            var periodTo = new DateTime(Period.Year, Period.Month, 1).AddMonths(1).AddDays(-1);

            //
            var EmployeeUsers = await _context.EmployeesUsers
                .Include(eu => eu.Employee)
                .ThenInclude(e => e.BasicPay)
                .Where(eu => eu.Start <= periodTo &&
                             eu.End >= periodFrom).ToListAsync();
            //
            var payrollResults = new Collection<PayrollCluster>();
            
            //Read all surveys in approval status for the processed period
            // for any employee defined in the system

            var stats = _context.SurveyStatusHistory
                .Where(s => s.Status == Models.DoEko.Survey.SurveyStatus.Approval &&
                            s.Start >= periodFrom &&
                            s.Start <= periodTo);

            foreach (var item in stats)
            {
                //get contractId
                var ContractId = _context.Surveys
                    .Where(s => s.SurveyId == item.SurveyId)
                    .Select(s => s.Investment.ContractId).FirstOrDefault();

                //get employee for the user
                var User = EmployeeUsers.Where(eu => eu.UserId == item.UserId).FirstOrDefault();
                if (User == null)
                {
                    continue;
                }

                //get rate for the employee
                var basicRate = User.Employee.BasicPay.Where(bp => bp.Code == "BASE" &&
                                                                   bp.Start <= periodFrom &&
                                                                   bp.End >= periodFrom &&
                                                                   (( bp.ContractId.HasValue == true && bp.ContractId.Value == ContractId) ||
                                                                    ( bp.ContractId.HasValue == false)))
                                                      .OrderByDescending(bp => bp.ContractId).FirstOrDefault();

                var employeeResult = payrollResults.FirstOrDefault(w => w.EmployeeId == User.Employee.EmployeeId);
                if (employeeResult == null)
                {
                    employeeResult = new PayrollCluster()
                    {
                        EmployeeId = User.EmployeeId,
                        PeriodFor = periodFrom,
                        PeriodIn = periodIn,
                        SequenceNo = 1,
                        Results = new Collection<PayrollResult>()
                    };
                    payrollResults.Add(employeeResult);
                }
                if (basicRate == null)
                {
                    employeeResult.Results.Add(new PayrollResult
                    {
                        Code = "",
                        ShortDescription = "",
                        Rate = 0,
                        Number = 0,
                        Unit = WageTypeUnit.Item,
                        Amount = 0,
                        Currency = "",
                        SurveyId = item.SurveyId,
                        Comments = new Collection<PayrollComment> { new PayrollComment { Text = "Brak składnika wynagrodzenia" } }
                    });
                }
                employeeResult.Results.Add(new PayrollResult
                {
                    Code = basicRate.Code,
                    ShortDescription = basicRate.ShortDescription,
                    Rate = basicRate.Rate,
                    Number = 1,
                    Unit = basicRate.Unit,
                    Amount = basicRate.Rate * 1,
                    Currency = basicRate.Currency,
                    SurveyId = item.SurveyId,
                });
            }

            //save results
            try
            {
                _context.PayrollCluster.AddRange(payrollResults);
                int result = await _context.SaveChangesAsync();


            }
            catch (Exception exc)
            {
                return Ok(exc);
            }

            return Ok();
        }

    }
}