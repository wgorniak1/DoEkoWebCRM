using System;
using System.Collections.Generic;
using DoEko.Models.DoEko;
using DoEko.Models.DoEko.Survey;
using Microsoft.AspNetCore.Identity;
using DoEko.Models.Identity;

namespace DoEko.Controllers.Helpers
{
    public class StatusHelper
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DoEkoContext _context;


        public StatusHelper(UserManager<ApplicationUser> userManager, DoEkoContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public ApplicationUser User { get; set; }

        public bool SetSurveyStatus(Survey survey, SurveyStatus oldStatus, SurveyStatus newStatus)
        {
            _context.Entry(survey).Reference(s => s.Investment).Load();
            _context.Entry(survey.Investment).Collection(i => i.Surveys).Load();
            _context.Entry(survey.Investment).Reference(i => i.Contract).Load();
            _context.Entry(survey.Investment.Contract).Reference(c => c.Project).Load();



            return true;
        }

        public bool SetInvestmentStatus(Investment investment)
        {
            return true;
        }
    }
}
