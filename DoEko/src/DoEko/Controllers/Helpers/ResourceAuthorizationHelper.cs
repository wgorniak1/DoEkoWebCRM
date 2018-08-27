using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoEko.Models.Identity;

namespace DoEko.Controllers.Helpers
{
    public class ResourceAuthorizationHelper
    {
        private readonly DoEko.Models.DoEko.DoEkoContext _context;

        public ResourceAuthorizationHelper(DoEko.Models.DoEko.DoEkoContext context)
        {
            _context = context;
        }

        public Task<bool> CheckAuthAsync(ApplicationUser applicationUser, AccessType accessType, ResourceType resourceType, int? id = null, Guid? guid = null)
        {
            return Task.Factory.StartNew( () => CheckAuth(applicationUser, accessType, resourceType, id, guid));
        }

        private bool CheckAuth(ApplicationUser applicationUser, AccessType accessType, ResourceType resourceType, int? id, Guid? guid)
        {
            switch (resourceType)
            {
                case ResourceType.Project:
                    id = accessType == AccessType.Create ? 1 : id;

                    return applicationUser.Projects.Any(p => p.ProjectId == id.Value && p.AccessType == accessType);
                case ResourceType.Contract:
                    return true;
                case ResourceType.Investment:
                    return true;
                case ResourceType.Survey:
                    return true;
                default:
                    return true;
            }

        }

        public Task<bool> CheckStructAuthAsync(ApplicationUser applicationUser, AccessType accessType, ResourceType resourceType, int? id = null, Guid? guid = null)
        {
            return Task.Factory.StartNew(() => CheckStructAuth(applicationUser, accessType, resourceType, id, guid));
        }

        private bool CheckStructAuth(ApplicationUser applicationUser, AccessType accessType, ResourceType resourceType, int? id = null, Guid? guid = null)
        {
            switch (resourceType)
            {
                case ResourceType.Project:
                    //Create authorization is not restricted to any specific object id.
                    //In the user auth assignment we set id = 1 if accesstype = create
                    id = accessType == AccessType.Create ? 1 : id;

                    if (!applicationUser.Projects.Any(p => p.ProjectId == id.Value && p.AccessType == accessType))
                    {
                        int? parentProjectId = _context.Projects.Where(p => p.ProjectId == id).Select(p => p.ParentProjectId).Single();

                        return parentProjectId.HasValue == true ? CheckStructAuth(applicationUser, accessType, ResourceType.Project, parentProjectId) : false;
                    }
                    return  true;
                case ResourceType.Contract:
                    try
                    {
                        int projectId = _context.Contracts.Where(c => c.ContractId == id && c.ProjectId != 0).Select(c => c.ProjectId).Single();

                        return CheckStructAuth(applicationUser, accessType, ResourceType.Project, projectId);

                    }
                    catch (Exception)
                    {
                        //can't find contract = not authorized
                        return false;
                    }
                case ResourceType.Investment:
                    try
                    {
                        int contractId = _context.Investments.Where(i => i.InvestmentId == guid && i.ContractId != 0).Select(i => i.ContractId).Single();

                        return CheckStructAuth(applicationUser, accessType, ResourceType.Contract, contractId);

                    }
                    catch (Exception)
                    {
                        //can't find investment = not authorized
                        return false;
                    }
                case ResourceType.Survey:
                    try
                    {
                        Guid investmentId = _context.Surveys.Where(s => s.SurveyId == guid).Select(s => s.InvestmentId).Single();

                        return CheckStructAuth(applicationUser, accessType, ResourceType.Investment, null ,investmentId);

                    }
                    catch (Exception)
                    {
                        //can't find survey = not authorized
                        return false;
                    }
                default:
                    return false;
            }
        }
    }
}
