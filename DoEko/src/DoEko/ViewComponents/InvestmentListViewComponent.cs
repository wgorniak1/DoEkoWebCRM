using DoEko.Controllers;
using DoEko.Controllers.Helpers;
using DoEko.Models.DoEko;
using DoEko.Models.DoEko.Addresses;
using DoEko.Models.Identity;
using DoEko.ViewComponents.ViewModels;
using DoEko.ViewModels.InvestmentViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DoEko.ViewComponents
{
    [ViewComponent]
    public class InvestmentListViewComponent : ViewComponent
    {
        private DoEkoContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public InvestmentListViewComponent(DoEkoContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync(InvestmentListViewModel model)
        {
            try
            {
                ApplicationUser _user = await _userManager.GetUserAsync(UserClaimsPrincipal);
                _user = _userManager.Users.Include(u => u.Projects).Single(u => u.Id == _user.Id);

                IQueryable<InvestmentOwner> qry;

                qry = _context.InvestmentOwners
                    .Where(io => _user.ProjectIds.Any(id => id == io.Investment.Contract.ProjectId))
                    .Include(io => io.Investment).ThenInclude(i => i.Address).ThenInclude(a=>a.Commune)
                    .Include(io => io.Investment).ThenInclude(i=> i.Surveys)
                    .Include(io => io.Owner);

                // if (model.filtering.* != null) qry = qry.where(io=>io.* = model.filtering.*)
                qry = ApplyFiltering(model.Filtering, qry);
                // qry = qry.ordeby / orderdescendingby(io=>io.*)
                qry = ApplySorting(model.Sorting, qry);
                //
                model.Paging.Calculate(qry.Count());
                //
                qry = ApplyPaging(model.Paging, qry);
                //Finally select elements
                model.List = await qry.ToListAsync();

                if (model.Filtering.UserId != Guid.Empty)
                {
                    return View("InspectorInvestments", model);
                }
                else
                {
                    return View("UnassignedInvestments", model);
                }
                //return View("InspectorInvestmentsUnassigned", model);
                //return View("AdminInvestments", model);
                //return View("UserInvestments", model);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private IQueryable<InvestmentOwner> ApplyPaging(InvestmentListPaging paging, IQueryable<InvestmentOwner> qry)
        {
            if (paging.PageSize != 0)
            {
                int rowsToSkip = (paging.CurrentNumber - 1) * (int)paging.PageSize;

                return qry.Skip(rowsToSkip).Take((int)paging.PageSize);
            }
            else
                return qry;
        }

        private IQueryable<InvestmentOwner> ApplySorting(InvestmentListSorting sorting, IQueryable<InvestmentOwner> query)
        {
            var sortOptions = sorting.sortBy.Split('_');
            //[0] = fieldname
            //[1] = direction up or down

            if (sortOptions == null)
            {
                return query;
            }

            if (sortOptions[0].Equals(nameof(Address)))
            {
                if (sortOptions[1].Equals(InvestmentListSorting.postfixUp.Remove(0, 1)))
                    return query.OrderBy(qry => qry.Investment.Address.Commune.FullName)
                                .ThenBy(qry => qry.Investment.Address.City)
                                .ThenBy(qry => qry.Investment.Address.Street)
                                .ThenBy(qry => qry.Investment.Address.BuildingNo)
                                .ThenBy(qry => qry.Investment.Address.ApartmentNo);
                else if (sortOptions[1].Equals(InvestmentListSorting.postfixDn.Remove(0, 1)))
                    return query.OrderByDescending(qry => qry.Investment.Address.Commune.FullName)
                                .ThenByDescending(qry => qry.Investment.Address.City)
                                .ThenByDescending(qry => qry.Investment.Address.Street)
                                .ThenByDescending(qry => qry.Investment.Address.BuildingNo)
                                .ThenByDescending(qry => qry.Investment.Address.ApartmentNo);
                else
                    return query;
            }
            else if (sortOptions[0].Equals(nameof(Investment.InvestmentOwners)))
            {
                if (sortOptions[1].Equals(InvestmentListSorting.postfixUp.Remove(0, 1)))
                    return query.OrderBy(qry => qry.Owner.PartnerName2).ThenBy(qry => qry.Owner.PartnerName1);
                else if (sortOptions[1].Equals(InvestmentListSorting.postfixDn.Remove(0, 1)))
                    return query.OrderByDescending(qry => qry.Owner.PartnerName2).ThenByDescending(qry => qry.Owner.PartnerName1);
                else
                    return query;
            }
            else if (sortOptions[0].Equals("Investment.Status"))
            {
                if (sortOptions[1].Equals(InvestmentListSorting.postfixUp.Remove(0, 1)))
                    return query.OrderBy(qry => qry.Investment.Status);
                else if (sortOptions[1].Equals(InvestmentListSorting.postfixDn.Remove(0, 1)))
                    return query.OrderByDescending(qry => qry.Investment.Status);
                else
                    return query;

            }
            else if (sortOptions[0].Equals("Investment.InspectionStatus"))
            {
                if (sortOptions[1].Equals(InvestmentListSorting.postfixUp.Remove(0, 1)))
                    return query.OrderBy(qry => qry.Investment.InspectionStatus);
                else if (sortOptions[1].Equals(InvestmentListSorting.postfixDn.Remove(0, 1)))
                    return query.OrderByDescending(qry => qry.Investment.InspectionStatus);
                else
                    return query;

            }
            else if (sortOptions[0].Equals("Investment.Surveys"))
            {
                if (sortOptions[1].Equals(InvestmentListSorting.postfixUp.Remove(0, 1)))
                    return query.OrderBy(qry => qry.Investment.Surveys.Count);
                else if (sortOptions[1].Equals(InvestmentListSorting.postfixDn.Remove(0, 1)))
                    return query.OrderByDescending(qry => qry.Investment.Surveys.Count);
                else
                    return query;

            }
            else
            {
                PropertyInfo property;
                if (sortOptions[0].Contains('.'))
                {
                    property = typeof(InvestmentOwner)
                                .GetProperty(sortOptions[0].Split('.')[0]);
                    property = property.PropertyType
                                .GetProperty(sortOptions[0].Split('.')[1]);
                }
                else
                {
                    property = typeof(InvestmentOwner).GetProperty(sortOptions[0]);
                }

                if (sortOptions[1].Equals(InvestmentListSorting.postfixUp.Remove(0,1)))
                    return query.OrderBy(qry => property.GetValue(qry,null));
                else if (sortOptions[1].Equals(InvestmentListSorting.postfixDn.Remove(0,1)))
                    return query.OrderByDescending(qry => property.GetValue(qry,null));
                else
                    return query;    
            }
        }

        private IQueryable<InvestmentOwner> ApplyFiltering(InvestmentListFilter filter, IQueryable<InvestmentOwner> query)
        {
            IQueryable<InvestmentOwner> qry = query;
            if (filter.ProjectId != 0)
                qry = qry.Where(i => i.Investment.Contract.ProjectId == filter.ProjectId);
            if (filter.ContractId != 0)
                qry = qry.Where(i => i.Investment.ContractId == filter.ContractId);
            if (filter.FilterByInspector == true)
            {
                //if userId == empty - unassigned investments
                //if userid != empty - assigned investments
                if (filter.UserId != Guid.Empty)
                    qry = qry.Where(i => i.Investment.InspectorId == filter.UserId);
                else
                    qry = qry.Where(i => i.Investment.InspectorId == null);
            }
            if (filter.Status != 0)
                qry = qry.Where(i => i.Investment.InspectionStatus == filter.Status);
            if (!string.IsNullOrEmpty(filter.CommuneId))
                qry = qry.Where(i => 
                i.Investment.Address.StateId == int.Parse(filter.CommuneId.Split('_')[0]) &&
                i.Investment.Address.DistrictId == int.Parse(filter.CommuneId.Split('_')[1]) &&
                i.Investment.Address.CommuneId == int.Parse(filter.CommuneId.Split('_')[2]) &&
                i.Investment.Address.CommuneType == (CommuneType)Enum.Parse(typeof(CommuneType),filter.CommuneId.Split('_')[3]));
            if (!string.IsNullOrEmpty(filter.City))
                qry = qry.Where(i => i.Investment.Address.City == filter.City);
            if (!string.IsNullOrEmpty(filter.FreeText))
                qry = qry.Where(i => 
                i.Investment.Address.SingleLine.ToLower().Contains(filter.FreeText.ToLower())
                ||
                (i.Owner.PartnerName1 + i.Owner.PartnerName2).ToLower().Contains(filter.FreeText.ToLower())
                );

            return qry;
        }
    }
}
