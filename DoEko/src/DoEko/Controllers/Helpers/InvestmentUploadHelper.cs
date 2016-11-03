using DoEko.Models;
using DoEko.Models.DoEko;
using DoEko.Models.DoEko.Addresses;
using DoEko.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Controllers.Helpers
{
    public enum InvestmentUploadRecord
    {
        [Display(Name = "OZE - C.O.")]
        SurveyCHType = 3,
        [Display(Name = "OZE - C.W.U.")]
        SurveyHWType = 4,
        [Display(Name = "OZE - Energia elektr.")]
        SurveyENType = 5,
        [Display(Name = "Właściciel - województwo")]
        OwnerState = 6,
        [Display(Name = "Właściciel - powiat")]
        OwnerDistrict = 7,
        [Display(Name = "Właściciel - gmina")]
        OwnerCommune = 8,
        [Display(Name = "Właściciel - kod poczty")]
        OwnerPostal = 9,
        [Display(Name = "Właściciel - miasto")]
        OwnerCity = 10,
        [Display(Name = "Właściciel - ulica")]
        OwnerStreet = 11,
        [Display(Name = "Właściciel - nr domu")]
        OwnerHouse = 12,
        [Display(Name = "Właściciel - nr mieszk")]
        OwnerApart = 13,
        [Display(Name = "Właściciel - Imię")]
        OwnerFirstName = 14,
        [Display(Name = "Właściciel - Nazwisko")]
        OwnerLastName = 15,
        [Display(Name = "Inwestycja - województwo")]
        InvestmentState = 16,
        [Display(Name = "Inwestycja - powiat")]
        InvestmentDistrict = 17,
        [Display(Name = "Inwestycja - gmina")]
        InvestmentCommune = 18,
        [Display(Name = "Inwestycja - kod poczty")]
        InvestmentPostal = 19,
        [Display(Name = "Inwestycja - miejscowość")]
        InvestmentCity = 20,
        [Display(Name = "Inwestycja - ulica")]
        InvestmentStreet = 21,
        [Display(Name = "Inwestycja - nr domu")]
        InvestmentHouse = 22,
        [Display(Name = "Inwestycja - Nr mieszkania")]
        InvestmentApart = 23,
        [Display(Name = "Inwestycja - nr działki")]
        InvestmentPlotNumber = 24,
        [Display(Name = "Inwestycja - nr ks. wieczystej")]
        InvestmentLandRegister = 25,
        [Display(Name = "Właściciel - telefon")]
        OwnerPhone = 26,
        [Display(Name = "Właściciel - adres e-mail")]
        OwnerMail = 27,
  //= 28,
  //= 29,
        [Display(Name = "Adres Inwestycji")]
        InvestmentAddress = 999,
        [Display(Name = "Adres Właściciela")]
        OwnerAddress = 998,
        [Display(Name = "Dane Właściela")]
        OwnerDetails = 997,
        [Display(Name = "Rodzaje OZE")]
        SurveyDetails = 996,

    }

    public class InvestmentUploadHelper
    {
        /// <summary>
        /// Field separator used in CSV file.
        /// </summary>
        public const string FieldSeparator = ";";
        /// <summary>
        /// Last row to be skipped when processing CSV records.
        /// </summary>
        public const int HeaderLastRow = 1;
        public const int MinColumnCount = 30;

        private DoEkoContext _context;
        /// <summary>
        /// Default ID of country, that is used when filling address data
        /// </summary>
        private int _defaultCountryId;
        /// <summary>
        /// 
        /// </summary>
        private string[] _RSETypeNamesCentralHeating;
        /// <summary>
        /// 
        /// </summary>
        private string[] _RSETypeNamesHotWater;
        /// <summary>
        /// 
        /// </summary>
        private string[] _RSETypeNamesEnergy;

        private string[] _CommuneTypeNames;
        private CommuneType[] _CommuneTypes;
        private string[] _record;

        public InvestmentUploadHelper(DoEkoContext context)
        {
            _context = context;
            _defaultCountryId = context.Countries.Single(c => c.Key == "PL").CountryId;

            SurveyRSETypeCentralHeating[] CHvalues = (SurveyRSETypeCentralHeating[])Enum.GetValues(typeof(SurveyRSETypeCentralHeating));
            _RSETypeNamesCentralHeating = CHvalues.Select(e => e.DisplayName()).ToArray();

            SurveyRSETypeHotWater[] HWvalues = (SurveyRSETypeHotWater[])Enum.GetValues(typeof(SurveyRSETypeHotWater));
            _RSETypeNamesHotWater = HWvalues.Select(e => e.DisplayName()).ToArray();

            SurveyRSETypeEnergy[] ENvalues = (SurveyRSETypeEnergy[])Enum.GetValues(typeof(SurveyRSETypeEnergy));
            _RSETypeNamesEnergy = ENvalues.Select(e => e.DisplayName()).ToArray();

            _CommuneTypes = (CommuneType[])Enum.GetValues(typeof(CommuneType));
            _CommuneTypeNames = _CommuneTypes.Select(ct => ct.DisplayName()).ToArray();
        }
        public string Record
        {
            set
            {
                _record = value.Split(FieldSeparator.ToCharArray());
                if (_record.Length < MinColumnCount)
                {
                    throw new InvestmentUploadException("", "", "Nieprawidłowa liczba kolumn.");
                }
                for (int i = 0; i < _record.Length; i++)
                {
                    _record[i] = _record[i].Trim();
                }
            }
            get
            {
                return string.Join(FieldSeparator, _record);
            }
        }
        public int ContractId { set; get; }

        public Investment ParseInvestment()
        {
            Investment _inv = new Investment();
            InvestmentUploadRecord cellNumber = InvestmentUploadRecord.SurveyDetails;
            string cellValue = "";

            try
            {   
                //PLOT NUMBER
                cellNumber = InvestmentUploadRecord.InvestmentPlotNumber;
                cellValue = _record[(int)cellNumber];

                _inv.PlotNumber = cellValue;

                //LANDREGISTER NUMBER
                cellNumber = InvestmentUploadRecord.InvestmentLandRegister;
                cellValue = _record[(int)cellNumber];

                _inv.LandRegisterNo = cellValue;

                _inv.Status = InvestmentStatus.Initial;
                _inv.InspectionStatus = InspectionStatus.NotExists;
                _inv.InvestmentId = Guid.NewGuid();
                _inv.ContractId = this.ContractId;
                return _inv;
            }
            catch (Exception)
            {

                throw new InvestmentUploadException(cellNumber.DisplayName(),cellValue);
            }

        }
        public ICollection<Survey> ParseSurveys()
        {
            ICollection<Survey> surveys = new Collection<Survey>();

            InvestmentUploadRecord cellNumber = InvestmentUploadRecord.SurveyDetails;
            string cellValue = "";

            try
            {
                cellNumber = InvestmentUploadRecord.SurveyCHType;
                cellValue = _record[(int)cellNumber];

                if (!string.IsNullOrEmpty(cellValue) && _RSETypeNamesCentralHeating.Contains(cellValue))
                {
                    surveys.Add(new SurveyCentralHeating {
                        SurveyId = Guid.NewGuid(),
                        Type = SurveyType.CentralHeating,
                        RSEType = (SurveyRSETypeCentralHeating)Enum.GetValues(typeof(SurveyRSETypeCentralHeating)).GetValue(_RSETypeNamesCentralHeating.ToList().IndexOf(cellValue))
                });
                }

                cellNumber = InvestmentUploadRecord.SurveyHWType;
                cellValue = _record[(int)cellNumber];

                if (!string.IsNullOrEmpty(cellValue) && _RSETypeNamesHotWater.Contains(cellValue))
                {
                    surveys.Add(new SurveyHotWater {
                        SurveyId = Guid.NewGuid(),
                        Type = SurveyType.HotWater,
                        RSEType = (SurveyRSETypeHotWater)Enum.GetValues(typeof(SurveyRSETypeHotWater)).GetValue(_RSETypeNamesHotWater.ToList().IndexOf(cellValue))
                    });
                }

                cellNumber = InvestmentUploadRecord.SurveyENType;
                cellValue = _record[(int)cellNumber] ?? string.Empty;

                if (!string.IsNullOrEmpty(cellValue) && _RSETypeNamesEnergy.Contains(cellValue))
                {
                    surveys.Add(new SurveyEnergy {
                        SurveyId = Guid.NewGuid(),
                        Type = SurveyType.Energy,
                        RSEType = (SurveyRSETypeEnergy)Enum.GetValues(typeof(SurveyRSETypeEnergy)).GetValue(_RSETypeNamesEnergy.ToList().IndexOf(cellValue))
                    });
                }
            }
            catch (Exception)
            {
                throw new InvestmentUploadException(cellNumber.DisplayName(), cellValue);
            }


            return surveys;
        }

        public Address ParseInvestmentAddress()
        {
            Address _addr = new Address();
            InvestmentUploadRecord cellNumber = InvestmentUploadRecord.InvestmentAddress;
            string cellValue = "";

            try
            {
                //COUNTRY
                _addr.CountryId = _defaultCountryId;
                // STATE
                cellNumber = InvestmentUploadRecord.InvestmentState;
                cellValue = _record[(int)cellNumber];
                _addr.StateId = _context.States.Single(s => cellValue.ToUpper().Contains(s.Text.ToUpper())).StateId;
                // DISTRICT
                cellNumber = InvestmentUploadRecord.InvestmentDistrict;
                cellValue = _record[(int)cellNumber];
                _addr.DistrictId = _context.Districts.Single(d =>
                                    d.StateId == _addr.StateId &&
                                    cellValue.ToUpper().Contains(d.Text.ToUpper())).DistrictId;
                // COMMUNE & COMMUNE TYPE
                //cellNumber = InvestmentUploadRecord.InvestmentCommuneType;
                //cellValue = _record[(int)cellNumber];
                //_addr.CommuneType = (CommuneType)Enum.ToObject(typeof(CommuneType), cellValue);
                cellNumber = InvestmentUploadRecord.InvestmentCommune;
                cellValue = _record[(int)cellNumber];

                _addr.CommuneType = _CommuneTypes[_CommuneTypeNames.ToList().IndexOf(cellValue.Substring(cellValue.IndexOf('(') + 1, cellValue.IndexOf(')') - cellValue.IndexOf('(') - 1))];
                    
                    //(CommuneType)Enum.Parse(typeof(CommuneType),
                    //cellValue.Substring(cellValue.IndexOf('(') + 1, cellValue.IndexOf(')') - cellValue.IndexOf('(') - 1));

                _addr.CommuneId = _context.Communes.Single(c =>
                                        c.StateId == _addr.StateId &&
                                        c.DistrictId == _addr.DistrictId &&
                                        c.Type == _addr.CommuneType &&
                                        cellValue.ToUpper().Contains(c.Text.ToUpper())).CommuneId;
                //POSTAL CODE
                cellNumber = InvestmentUploadRecord.InvestmentPostal;
                cellValue = _record[(int)cellNumber].GetNumbers();
                if (cellValue.Length < 5)
                {
                    throw new InvestmentUploadException(cellNumber.DisplayName(), cellValue);
                }
                _addr.PostalCode = cellValue.Substring(0, 2) + '-' + cellValue.Substring(2, 3);
                //CITY
                cellNumber = InvestmentUploadRecord.InvestmentCity;
                cellValue = _record[(int)cellNumber];
                _addr.City = cellValue.Substring(0, cellValue.Length > 50 ? 49 : cellValue.Length);
                //STREET
                cellNumber = InvestmentUploadRecord.InvestmentStreet;
                cellValue = _record[(int)cellNumber];
                _addr.Street = cellValue.Substring(0, cellValue.Length > 50 ? 49 : cellValue.Length);
                //BUILDING
                cellNumber = InvestmentUploadRecord.InvestmentHouse;
                cellValue = _record[(int)cellNumber];
                _addr.BuildingNo = cellValue.ToUpper().Substring(0, cellValue.Length > 10 ? 9 : cellValue.Length);
                //APARTMENT
                cellNumber = InvestmentUploadRecord.InvestmentApart;
                cellValue = _record[(int)cellNumber];
                _addr.ApartmentNo = cellValue.ToUpper().Substring(0, cellValue.Length > 11 ? 10 : cellValue.Length);

                return _addr;
            }
            catch (InvestmentUploadException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new InvestmentUploadException(cellNumber.DisplayName(), cellValue);
            }

        }

        public Address ParseOwnerAddress()
        {
            Address _addr = new Address();
            string cellValue = "";
            InvestmentUploadRecord cellNumber = InvestmentUploadRecord.OwnerAddress;

            try
            {
                //COUNTRY
                _addr.CountryId = _defaultCountryId;
                // STATE
                cellNumber = InvestmentUploadRecord.OwnerState;
                cellValue = _record[(int)cellNumber];
                _addr.StateId = _context.States.Single(s => cellValue.ToUpper().Contains(s.Text.ToUpper())).StateId;
                // DISTRICT
                cellNumber = InvestmentUploadRecord.OwnerDistrict;
                cellValue = _record[(int)cellNumber];
                _addr.DistrictId = _context.Districts.Single(d =>
                                    d.StateId == _addr.StateId &&
                                    cellValue.ToUpper().Contains(d.Text.ToUpper())).DistrictId;
                // COMMUNE & COMMUNE TYPE
                cellNumber = InvestmentUploadRecord.OwnerCommune;
                cellValue = _record[(int)cellNumber];

                //COMMUNE TYPE
                //cellNumber = InvestmentUploadRecord.OwnerCommuneType;
                //cellValue = _record[(int)cellNumber];
                _addr.CommuneType = _CommuneTypes[_CommuneTypeNames.ToList().IndexOf(cellValue.Substring(cellValue.IndexOf('(') + 1, cellValue.IndexOf(')') - cellValue.IndexOf('(') - 1))];

                //_addr.CommuneType = (CommuneType)Enum.Parse(typeof(CommuneType), 
                //    cellValue.Substring(cellValue.IndexOf('(') + 1,cellValue.IndexOf(')') - cellValue.IndexOf('(') - 1));

                _addr.CommuneId = _context.Communes.Single(c =>
                                        c.StateId == _addr.StateId &&
                                        c.DistrictId == _addr.DistrictId &&
                                        c.Type == _addr.CommuneType &&
                                        cellValue.ToUpper().Contains(c.Text.ToUpper())).CommuneId;
                //POSTAL CODE
                cellNumber = InvestmentUploadRecord.OwnerPostal;
                cellValue = _record[(int)cellNumber].GetNumbers();
                if (cellValue.Length < 5)
                {
                    throw new InvestmentUploadException(cellNumber.DisplayName(), cellValue);
                }
                _addr.PostalCode = cellValue.Substring(0, 2) + '-' + cellValue.Substring(2, 3);
                //CITY
                cellNumber = InvestmentUploadRecord.OwnerCity;
                cellValue = _record[(int)cellNumber];
                _addr.City = cellValue.Substring(0, cellValue.Length > 50 ? 49 : cellValue.Length);
                //STREET
                cellNumber = InvestmentUploadRecord.OwnerStreet;
                cellValue = _record[(int)cellNumber];
                _addr.Street = cellValue.Substring(0, cellValue.Length > 50 ? 49 : cellValue.Length);
                //BUILDING
                cellNumber = InvestmentUploadRecord.OwnerHouse;
                cellValue = _record[(int)cellNumber];
                _addr.BuildingNo = cellValue.ToUpper().Substring(0, cellValue.Length > 10 ? 9 : cellValue.Length);
                //APARTMENT
                cellNumber = InvestmentUploadRecord.OwnerApart;
                cellValue = _record[(int)cellNumber];
                _addr.ApartmentNo = cellValue.ToUpper().Substring(0, cellValue.Length > 11 ? 10 : cellValue.Length);
            }
            catch (Exception)
            {
                throw new InvestmentUploadException(cellNumber.DisplayName(), cellValue);
            }
            return _addr;
        }

        public BusinessPartner ParseInvestmentOwner()
        {
            BusinessPartnerPerson _owner = new BusinessPartnerPerson();
            InvestmentUploadRecord cellNumber = InvestmentUploadRecord.OwnerDetails;
            String cellValue = " ";

            try
            {
                //FIRST NAME
                cellNumber = InvestmentUploadRecord.OwnerFirstName;
                cellValue = _record[(int)cellNumber];
                _owner.FirstName = cellValue.Substring(0, cellValue.Length >= 30 ? 29 : cellValue.Length);
                //LAST NAME
                cellNumber = InvestmentUploadRecord.OwnerLastName;
                cellValue = _record[(int)cellNumber];
                _owner.LastName = cellValue.Substring(0, cellValue.Length >= 30 ? 29 : cellValue.Length);
                //E-MAIL
                cellNumber = InvestmentUploadRecord.OwnerMail;
                cellValue = _record[(int)cellNumber];
                _owner.Email = cellValue.Trim();
                //PHONE
                cellNumber = InvestmentUploadRecord.OwnerPhone;
                cellValue = _record[(int)cellNumber];
                int len = cellValue.GetNumbers().Length;
                if (len > 0 && len != 9)
                {
                    throw new InvestmentUploadException(cellNumber.DisplayName(), cellValue);
                }
                _owner.PhoneNumber = cellValue.AsPhoneNumber();

            }
            catch (Exception)
            {
                throw new InvestmentUploadException(cellNumber.DisplayName(), cellValue);
            }

            return _owner;
        }

    }

    public class InvestmentUploadException : SystemException
    {
        public InvestmentUploadException() : base() { }
        public InvestmentUploadException(string fieldname, string fieldvalue)
            : this(fieldname, fieldvalue, "Nieprawidłowa wartość '{1}' w polu '{0}'") { }

        public InvestmentUploadException(string fieldname, string fieldvalue, string message) : base(message)
        {
            this.Fieldname = fieldname;
            this.Fieldvalue = fieldvalue;
        }

        public string Fieldname { get; }
        public string Fieldvalue { get; }
        public override string Message
        {
            get
            {
                return string.Format(base.Message, Fieldname, Fieldvalue);
            }
        }
    }
}
