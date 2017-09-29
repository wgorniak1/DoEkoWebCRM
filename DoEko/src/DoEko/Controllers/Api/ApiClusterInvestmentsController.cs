using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DoEko.Models.DoEko.ClusterImport;
using DoEko.Models.DoEko;
using Microsoft.AspNetCore.Authorization;
using DoEko.ViewModels.API.ClusterInvestmentViewModels;
using DoEko.Models.DoEko.Addresses;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using DoEko.Controllers.Helpers;

namespace DoEko.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/v1/ClusterInvestments")]
    public class ApiClusterInvestmentsController : Controller
    {
        private readonly DoEkoContext _context;

        
        public ApiClusterInvestmentsController(DoEkoContext context)
        {
            _context = context;
        }
        
        // POST: api/ApiClusterInvestments/Create
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] ClusterInvestmentVM investment)
        {
            if (investment == null ||
                (investment.Person == null && 
                 investment.Organization == null) ||
                 (investment.ExistingInstallation == null &&
                  investment.NewInstallationFarm == null &&
                  investment.NewInstallationPros == null))
            {
                ModelState.AddModelError("formularz", "Błędna struktura formularza");
                //something went really wrong
                return BadRequest(ModelState);
            }

            //Validate contract
            CheckContract(investment.ContractId);

            //validate address
            ValidateAddress(investment.Person != null ? investment.Person.Address : investment.Organization.Address);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //convert to db model
            ClusterInvestment model = ClusterInvestmentHelper.ToModel(investment);

            //check duplicated entries
            CheckDuplicates(model);

            _context.ClusterInvestments.Add(model);
            int result = await _context.SaveChangesAsync();
            investment.ClusterInvestmentId = model.ClustInvestmentId;

            return CreatedAtAction("Create", new { id = model.ClustInvestmentId}, investment);
        }

        private void CheckContract(int contractId)
        {
            if (!_context.Contracts.Any( c => c.ContractId == contractId ))
            {
                ModelState.AddModelError("Gmina", "Brak możliwości dołączenia do wybranej gminy");
            }
        }

        private void CheckDuplicates(ClusterInvestment ci)
        {
            if (_context.ClusterInvestments.Any(i => i.Address == ci.Address))
            {
                ModelState.AddModelError(nameof(ci.Address), "Istnieje już zgłoszenie dla tego adresu");
            }
            
            //var nip = _context.ClusterInvestments.Any(i => i.TaxId == ci.TaxId);

        }

        private void ValidateAddress(Address address)
        {
            if (!_context.States.Any(s => s.StateId == address.StateId))
            {
                ModelState.AddModelError(nameof(address.StateId), "Błędny kod województwa");
            }
            else if (!_context.Districts.Any(d => d.StateId == address.StateId && d.DistrictId == address.DistrictId))
            {
                ModelState.AddModelError(nameof(address.DistrictId), "Błędny kod powiatu");
            }
            else if (!_context.Communes.Any(c => c.StateId == address.StateId &&
                                                c.DistrictId == address.DistrictId &&
                                                c.CommuneId == address.CommuneId))
            {
                ModelState.AddModelError(nameof(address.CommuneId), "Błędny kod gminy");
            }
            else if (!_context.Communes.Any(c => c.StateId == address.StateId &&
                                                c.DistrictId == address.DistrictId &&
                                                c.CommuneId == address.CommuneId &&
                                                c.Type == address.CommuneType))
            {
                ModelState.AddModelError(nameof(address.CommuneType), "Błędny kod typu gminy");
            }
        }
    }
}
