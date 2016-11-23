using DoEko.Models.DoEko;
using DoEko.Models.DoEko.Survey;
using DoEko.Models.DoEko.Addresses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DoEko.ViewModels.SurveyViewModels
{
    public class DetailsCHViewModel
    {
        public DetailsCHViewModel()
        {

        }
        public DetailsCHViewModel(SurveyCentralHeating survey)
        {
            InvestmentAddress = survey.Investment.Address;
            InvestmentAddress.CommuneId = InvestmentAddress.CommuneId * 10 + (int)InvestmentAddress.CommuneType;
            Owners = survey.Investment.InvestmentOwners.Select(io => new InvestmentOwnerVM()
            {
                InvestmentId = io.InvestmentId,
                InvestmentOwnerId = io.OwnerId,
                OwnershipType = io.OwnershipType,
                Sponsor = io.Sponsor,
                Person = (io.Owner.GetType() == typeof(BusinessPartnerPerson)) ? (BusinessPartnerPerson)io.Owner : null,
                Unit = (io.Owner.GetType() == typeof(BusinessPartnerEntity)) ? (BusinessPartnerEntity)io.Owner : null,
                SameAddress = (io.Investment.AddressId == io.Owner.AddressId),
                Address = io.Owner.Address

            }).ToList();

            foreach (var item in Owners)
            {
                item.Address.CommuneId = item.Address.CommuneId * 10 + (int)item.Address.CommuneType;
            }

            Survey = survey;
            if (survey.AirCondition == null)
            {
                survey.AirCondition = new SurveyDetAirCond();
            }
            if (survey.Audit == null)
            {
                survey.Audit = new SurveyDetEnergyAudit();
            }
            if (survey.BathRoom == null)
            {
                survey.BathRoom = new SurveyDetBathroom();
            }
            if (survey.BoilerRoom == null)
            {
                survey.BoilerRoom = new SurveyDetBoilerRoom();
            }
            if (survey.Building == null)
            {
                survey.Building = new SurveyDetBuilding();
            }
            if (survey.Ground == null)
            {
                survey.Ground = new SurveyDetGround();
            }
            if (survey.RoofPlanes == null)
            {
                survey.RoofPlanes = new List<SurveyDetRoof>();
                survey.RoofPlanes.Add(new SurveyDetRoof() { SurveyId = survey.SurveyId });
            }
            
        }
        public SurveyCentralHeating Survey { get; set; }
        public Address InvestmentAddress { get; set; }
        public IList<InvestmentOwnerVM> Owners { get; set; }

        
        //        public ICollection<BusinessPartnerPerson> Owners { get; set; }

        //public Investment Investment { get; set; }

    }

    public class InvestmentOwnerVM
    {
        public Guid InvestmentOwnerId { get; set; }
        public Guid InvestmentId { get; set; }
        [Display(Name = "Inwestor")]
        public bool Sponsor { get; set; }
        public OwnershipType OwnershipType { get; set; }
        [Display(Name = "Adres taki sam jak inwest.")]
        public bool SameAddress { get; set; }
        public Address Address { get; set; }
        //public Type OwnerType { get; set; }
        public BusinessPartnerPerson Person { get; set; }
        public BusinessPartnerEntity Unit { get; set; }
    }
}
