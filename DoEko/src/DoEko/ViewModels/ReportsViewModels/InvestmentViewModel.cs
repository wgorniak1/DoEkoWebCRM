using DoEko.Controllers.Helpers;
using DoEko.Models.DoEko;
using DoEko.Models.DoEko.Survey;
using DoEko.Services;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.ViewModels.ReportsViewModels
{
    public class InvestmentViewModel : Investment
    {
        private RSEPriceHelper _rsePrice;
        private Survey _survey;

        public InvestmentViewModel(InvestmentViewModel inv) : this((Investment)inv)
        {
            Pictures = inv.Pictures;
            _rsePrice = inv._rsePrice;
        }
        public InvestmentViewModel(Investment inv)
        {
            this.Address = inv.Address;
            this.AddressId = inv.AddressId;
            this.BusinessActivity = inv.BusinessActivity;
            this.CentralHeatingFuel = inv.CentralHeatingFuel;
            this.CentralHeatingType = inv.CentralHeatingType;
            this.CentralHeatingTypeOther = inv.CentralHeatingTypeOther;
            this.CompletionYear = inv.CompletionYear;
            this.Contract = inv.Contract;
            this.ContractId = inv.ContractId;
            this.HeatedArea = inv.HeatedArea;
            this.HotWaterFuel = inv.HotWaterFuel;
            this.HotWaterType = inv.HotWaterType;
            this.InspectionStatus = inv.InspectionStatus;
            this.InspectorId = inv.InspectorId;
            this.InternetAvailable = inv.InternetAvailable;
            this.InvestmentId = inv.InvestmentId;
            this.InvestmentOwners = inv.InvestmentOwners;
            this.LandRegisterNo = inv.LandRegisterNo;
            this.NumberOfOccupants = inv.NumberOfOccupants;
            this.Payments = inv.Payments;
            this.UsableArea = inv.UsableArea;
            this.PlotAreaNumber = inv.PlotAreaNumber;
            this.PlotNumber = inv.PlotNumber;
            this.PriorityIndex = inv.PriorityIndex;
            this.Stage = inv.Stage;
            this.Status = inv.Status;
            this.Surveys = inv.Surveys;
            this.TotalArea = inv.TotalArea;
            this.Type = inv.Type;

            this.Investment = this;

        }
        public Uri Picture(string id)
        {
            try
            {
                return (id == "Picture0" || id == "Picture5" || id == "Picture10" ) ?
                    Pictures[InvestmentId][id] :
                    Pictures[Survey.SurveyId][id];
            }
            catch (Exception)
            {
                return null;
            }
        }

        [NotMapped]
        private Dictionary<Guid, Dictionary<string, Uri>> Pictures { get; set; }
        public void ReadPictures(IFileStorage _fileStorage)
        {
            this.Pictures = new Dictionary<Guid, Dictionary<string, Uri>>();

            //Investment
            try
            {
                var cont = _fileStorage.GetBlobContainer(EnuAzureStorageContainerType.Investment);
                var files = cont.ListBlobs(prefix: this.InvestmentId.ToString(), useFlatBlobListing: true).OfType<CloudBlockBlob>();

                Dictionary<string, Uri> tmpList = new Dictionary<string, Uri>();

                foreach (var file in files)
                {
                    var parts = file.Uri.ToString().Split('/').Reverse().ToArray();
                    if (parts[1].Contains("Picture"))
                    {
                        try
                        {
                            tmpList.Add(parts[1], file.Uri);
                        }
                        catch (Exception)
                        { }
                    }
                }

                Pictures.Add(InvestmentId, tmpList);
            }
            catch (Exception)
            {
                throw;
            }

            //Surveys
            foreach (var srv in this.Surveys)
            {
                try
                {
                    var cont = _fileStorage.GetBlobContainer(EnuAzureStorageContainerType.Survey);
                    var files = cont.ListBlobs(prefix: srv.SurveyId.ToString(), useFlatBlobListing: true).OfType<CloudBlockBlob>();

                    Dictionary<string, Uri> tmpList = new Dictionary<string, Uri>();

                    foreach (var file in files)
                    {
                        var parts = file.Uri.ToString().Split('/').Reverse().ToArray();
                        if (parts[1].Contains("Picture"))
                        {
                            try
                            {
                                tmpList.Add(parts[1], file.Uri);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }

                    Pictures.Add(srv.SurveyId, tmpList);
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public BusinessPartner FirstOwner
        {
            get
            {
                return this.InvestmentOwners != null && this.InvestmentOwners.Count > 0 ?
                    this.InvestmentOwners.First().Owner != null ?
                    this.InvestmentOwners.First().Owner : null : null;
            }
            private set
            {

            }
        }
        [NotMapped]
        /// <summary>
        /// 
        /// </summary>
        public Survey Survey
        {
            get => _survey;
            set { _survey = value; _rsePrice.Survey = value; }
        }
        [NotMapped]
        public InvestmentViewModel Investment { get; set; }

        internal void SetRSEPrice(DoEkoContext context)
        {
            this._rsePrice = new RSEPriceHelper(context, false, this.Contract.ProjectId);
        }

        public RSEPriceHelper RSEPrice { get { return this._rsePrice; } set { } }
    }
}
