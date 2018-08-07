using DoEko.Controllers.Extensions;
using DoEko.Models.DoEko;
using DoEko.Models.DoEko.Survey;
using DoEko.Services;
using DoEko.ViewModels.API.SurveyViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DoEko.Controllers.Helpers
{
    public class NeoExtractHelper
    {
        private readonly DoEkoContext _context;
        private readonly IFileStorage _fileStorage;
        private IQueryable<Investment> query;
        private int contractId;
        public NeoExtractHelper(DoEkoContext context, IFileStorage fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        public IQueryable<Investment> PrepareQuery(int contractId, Guid? investmentId, int maxHits)
        {
            this.contractId = contractId;

            var sl = _context.Surveys.AsQueryable();

            sl = sl.Where(s => s.Investment.ContractId == contractId &&
                               s.Investment.Status == InvestmentStatus.Initial &&
                               s.Investment.Calculate == true);

            sl = investmentId.HasValue ? sl.Where(s => s.InvestmentId == investmentId) : sl;

            sl = sl.OrderBy(s => s.InvestmentId);

            sl = sl.Include(s => s.Investment).ThenInclude(i => i.InvestmentOwners).ThenInclude(io => io.Owner).ThenInclude(o => o.Address)
                   .Include(s => s.Investment).ThenInclude(i => i.Address).ThenInclude(a => a.State)
                   .Include(s => s.Investment).ThenInclude(i => i.Address).ThenInclude(a => a.District)
                   .Include(s => s.Investment).ThenInclude(i => i.Address).ThenInclude(a => a.Commune)
                   .Include(s => s.AirCondition)
                   .Include(s => s.Audit)
                   .Include(s => s.BathRoom)
                   .Include(s => s.BoilerRoom)
                   .Include(s => s.Building)
                   .Include(s => s.Ground)
                   .Include(s => s.PlannedInstall)
                   .Include(s => s.RoofPlanes)
                   .Include(s => s.Wall);

            this.query = sl.ToList().Select(s => s.Investment).Distinct().Take(maxHits).AsQueryable();

            return this.query;

        }

        public async Task<string> CreateFile()
        {
            //convert to viewmodel -> datatable -> openxml excel
            MemoryStream report = query
                .SelectMany(i => i.Surveys)
                .Select(s => new SurveyNeoVM(s, _fileStorage))
                .ToList()
                .AsDataTable()
                .AsExcel();


            //Create file on AzureStorage
            string fileName = "OZEDoAnalizy_" + DateTime.Now.ToFileTime() + ".xlsx";
            
            var blob = (await _fileStorage.GetBlobContainerAsync(EnuAzureStorageContainerType.NeoDownloads))
                .GetDirectoryReference(this.contractId.ToString() + "/")
                .GetBlockBlobReference(fileName);
            
            report.Position = 0;
            await blob.UploadFromStreamAsync(report);

            return blob.Uri.AbsoluteUri;
        }
        public async Task<int> UpdateStatus()
        {
            foreach (var i in this.query)
            {
                i.Status = InvestmentStatus.InReview;
            }
            if (this.query.Count() > 0)
            {

                _context.UpdateRange(this.query);

                return await _context.SaveChangesAsync();

            }
            else return await Task<int>.Factory.StartNew(() => { return 0; });

        }
    }

    //public enum NeoExtractRecord
    //{
    //    [Display(Name = "ID INWESTYCJI")]
    //    col1,
    //    [Display(Name = "PRIORYTET")]
    //    col2,
    //    [Display(Name = "ID ANKIETY")]
    //    col3,
    //    [Display(Name = "TYP OZE")]
    //    col4,
    //    [Display(Name = "STATUS ANKIETY")]
    //    col5,
    //    [Display(Name = "INWEST - ADRES - WOJ.")]
    //    col6,
    //    [Display(Name = "INWEST - ADRES - POW.")]
    //    col7,
    //    [Display(Name = "INWEST - ADRES - GM.")]
    //    col8,
    //    [Display(Name = "INWEST - ADRES - KOD")]
    //    col9,
    //    [Display(Name = "INWEST - ADRES - MIEJSC")]
    //    col10,
    //    [Display(Name = "INWEST - ADRES - ULICA")]
    //    col11,
    //    [Display(Name = "INWEST - ADRES - NR BUD.")]
    //    col12,
    //    [Display(Name = "INWEST - ADRES - NR MIESZK")]
    //    col13,
    //    [Display(Name = "WŁAŚCICIEL 0")]
    //    col14,
    //    [Display(Name = "WŁAŚCICIEL ADRES 0")]
    //    col15,
    //    [Display(Name = "WŁAŚCICIEL TEL 0")]
    //    col16,
    //    [Display(Name = "WŁAŚCICIEL MAIL 0")]
    //    col17,
    //    [Display(Name = "WŁAŚCICIEL 1")]
    //    col18,
    //    [Display(Name = "WŁAŚCICIEL ADRES 1")]
    //    col19,
    //    [Display(Name = "WŁAŚCICIEL TEL 1")]
    //    col20,
    //    [Display(Name = "WŁAŚCICIEL MAIL 1")]
    //    col21,
    //    [Display(Name = "WŁAŚCICIEL 2")]
    //    col22,
    //    [Display(Name = "WŁAŚCICIEL ADRES 2")]
    //    col23,
    //    [Display(Name = "WŁAŚCICIEL TEL 2")]
    //    col24,
    //    [Display(Name = "WŁAŚCICIEL MAIL 2")]
    //    col25,
    //    [Display(Name = "INTERNET W M.INW.")]
    //    col26,
    //    [Display(Name = "RODZ. DZIAŁALN.")]
    //    col27,
    //    [Display(Name = "NR KS. WIECZ.")]
    //    col28,
    //    [Display(Name = "NR DZIAŁKI")]
    //    col29,
    //    [Display(Name = "PALIWO GŁ.CO")]
    //    col30,
    //    [Display(Name = "RODZAJ GŁ.CO")]
    //    col31,
    //    [Display(Name = "PALIWO GŁ. CW")]
    //    col32,
    //    [Display(Name = "RODZAJ GŁ. CW")]
    //    col33,
    //    [Display(Name = "STAN BUD.")]
    //    col34,
    //    [Display(Name = "ROK BUDOWY")]
    //    col35,
    //    [Display(Name = "L.MIESZKAŃCÓW")]
    //    col36,
    //    [Display(Name = "POW. UŻYTK.")]
    //    col37,
    //    [Display(Name = "POW. OGRZEWANA")]
    //    col38,
    //    [Display(Name = "POW. CAŁK.")]
    //    col39,
    //    [Display(Name = "RODZ.BUD.")]
    //    col40,
    //    [Display(Name = "UMOWA KOMPLEKS.")]
    //    col41,
    //    [Display(Name = "EE - DYSTRYBUTOR")]
    //    col42,
    //    [Display(Name = "EE - MOC PRZYŁ.")]
    //    col43,
    //    [Display(Name = "EE - RODZ. PRZYŁ.")]
    //    col44,
    //    [Display(Name = "EE - L. FAZ")]
    //    col45,
    //    [Display(Name = "EE - UZIEMIENIE")]
    //    col46,
    //    [Display(Name = "EE - UMIEJSC. LICZNIKA")]
    //    col47,
    //    [Display(Name = "EE - DOD. LICZNIK")]
    //    col48,
    //    [Display(Name = "EE - ROCZNE ZUŻYCIE")]
    //    col49,
    //    [Display(Name = "EE - ŚR. KOSZT/MC.")]
    //    col50,
    //    [Display(Name = "EE - PLANOWANA MOC")]
    //    col51,
    //    [Display(Name = "KOTŁOWNIA - ISTNIEJE")]
    //    col52,
    //    [Display(Name = "KOTŁOWNIA - SZER.DRZWI")]
    //    col53,
    //    [Display(Name = "KOTŁOWNIA - DŁUG.")]
    //    col54,
    //    [Display(Name = "KOTŁOWNIA - SZER.")]
    //    col55,
    //    [Display(Name = "KOTŁOWNIA - WYS.")]
    //    col56,
    //    [Display(Name = "KOTŁOWNIA - KUBATURA")]
    //    col57,
    //    [Display(Name = "KOTŁOWNIA - ISTN. INST. CW")]
    //    col58,
    //    [Display(Name = "KOTŁOWNIA - ISTN. CYRKULACJA")]
    //    col59,
    //    [Display(Name = "KOTŁOWNIA - ISTN. REDUKTOR C.")]
    //    col60,
    //    [Display(Name = "KOTŁOWNIA - ISTN.WOLNY PRZEW.WENT")]
    //    col61,
    //    [Display(Name = "KOTŁOWNIA - SUCHA I > 0 ST.")]
    //    col62,
    //    [Display(Name = "KOTŁOWNIA - ISTN. 3 UZIEM.GNIAZDA")]
    //    col63,
    //    [Display(Name = "KOTŁOWNIA - INST.400V")]
    //    col64,
    //    [Display(Name = "L.ŁAZIENEK")]
    //    col65,
    //    [Display(Name = "ISTN. WANNA")]
    //    col66,
    //    [Display(Name = "OBJ. WANNY")]
    //    col67,
    //    [Display(Name = "ISTN.PRYSZNIC")]
    //    col68,
    //    [Display(Name = "ZESTAW KLIENTA")]
    //    col69,
    //    [Display(Name = "CW - MOC")]
    //    col70,
    //    [Display(Name = "PIEC - MOC")]
    //    col71,
    //    [Display(Name = "PIEC - ROK PROD.")]
    //    col72,
    //    [Display(Name = "PIEC - PLAN. WYM.")]
    //    col73,
    //    [Display(Name = "CO GRZEJNIKI")]
    //    col74,
    //    [Display(Name = "CO GRZEJNIKI - TYP")]
    //    col75,
    //    [Display(Name = "CO PODLOG.")]
    //    col76,
    //    [Display(Name = "CO PODLOG. - PROCENT POW.")]
    //    col77,
    //    [Display(Name = "MAX TEMP. PIECA")]
    //    col78,
    //    [Display(Name = "ŚR.ROCZNE ZUŻYCIE CO")]
    //    col79,
    //    [Display(Name = "ŚR.ROCZNE KOSZTY CO")]
    //    col80,
    //    [Display(Name = "MECH.WENT.ISTN.")]
    //    col81,
    //    [Display(Name = "ISTN. KLIMAT.")]
    //    col82,
    //    [Display(Name = "KLIMAT.PLANOWANA")]
    //    col83,
    //    [Display(Name = "TYP INST.CHŁODZ.")]
    //    col84,
    //    [Display(Name = "DOD.ŹR.CIEPŁA")]
    //    col85,
    //    [Display(Name = "PAR.DOD.ŹR.CIEPŁA")]
    //    col86,
    //    [Display(Name = "CW - ISTN. ZASOBNIK")]
    //    col87,
    //    [Display(Name = "CW - OBJ. ZASOBNIKA")]
    //    col88,
    //    [Display(Name = "CW - POW. WĘŻ.")]
    //    col89,
    //    [Display(Name = "PLAN.POMPA BĘDZIE JEDYNYM ŹR.")]
    //    col90,
    //    [Display(Name = "TECHNOLOGIA WYKONANIA")]
    //    col91,
    //    [Display(Name = "MATERIAŁ ŚCIAN")]
    //    col92,
    //    [Display(Name = "GRUBOŚĆ ŚCIAN")]
    //    col93,
    //    [Display(Name = "IZOLACJA - RODZAJ")]
    //    col94,
    //    [Display(Name = "IZOLACJA - GRUBOŚĆ")]
    //    col95,
    //    [Display(Name = "KUBATURA BUD.")]
    //    col96,
    //    [Display(Name = "LOKALIZACJA INSTALACJI")]
    //    col97,
    //    [Display(Name = "INSTALACJA NA SCIANIE")]
    //    col98,
    //    [Display(Name = "PRZEZN. BUDYNKU")]
    //    col99,
    //    [Display(Name = "GRUNT - POW.")]
    //    col100,
    //    [Display(Name = "GRUNT - BYLY TEREN WOJSK")]
    //    col101,
    //    [Display(Name = "GRUNT - ISTN.INSTALACJA")]
    //    col102,
    //    [Display(Name = "GRUNT - INSTALACJA TYP")]
    //    col103,
    //    [Display(Name = "GRUNT - GRUZ,SKAŁY")]
    //    col104,
    //    [Display(Name = "GRUNT - NACHYLENIE")]
    //    col105,
    //    [Display(Name = "GRUNT - PODMOKŁY")]
    //    col106,
    //    [Display(Name = "POŁAĆ 0 TYP")]
    //    col107,
    //    [Display(Name = "POŁAĆ 0 WYS.BUD.")]
    //    col108,
    //    [Display(Name = "POŁAĆ 0 WYS.OKAPU")]
    //    col109,
    //    [Display(Name = "POŁAĆ 0 DŁ. DACHU")]
    //    col110,
    //    [Display(Name = "POŁAĆ 0 DŁ.KRAW.")]
    //    col111,
    //    [Display(Name = "POŁAĆ 0 DŁ.GRZBIETU")]
    //    col112,
    //    [Display(Name = "POŁAĆ 0 KĄT NACH.")]
    //    col113,
    //    [Display(Name = "POŁAĆ 0 DŁUG.")]
    //    col114,
    //    [Display(Name = "POŁAĆ 0 SZER.")]
    //    col115,
    //    [Display(Name = "POŁAĆ 0 POWIERZCHNIA")]
    //    col116,
    //    [Display(Name = "POŁAĆ 0 POKRYCIE")]
    //    col117,
    //    [Display(Name = "POŁAĆ 0 AZYMUT")]
    //    col118,
    //    [Display(Name = "POŁAĆ 0 OKNA")]
    //    col119,
    //    [Display(Name = "POŁAĆ 0 ŚWIETLIKI")]
    //    col120,
    //    [Display(Name = "POŁAĆ 0 KOMINY")]
    //    col121,
    //    [Display(Name = "POŁAĆ 0 INSTALACJA POD")]
    //    col122,
    //    [Display(Name = "POŁAĆ 0 INST.ODGROM")]
    //    col123,
    //    [Display(Name = "POŁAĆ 1 TYP")]
    //    col124,
    //    [Display(Name = "POŁAĆ 1 WYS.BUD.")]
    //    col125,
    //    [Display(Name = "POŁAĆ 1 WYS.OKAPU")]
    //    col126,
    //    [Display(Name = "POŁAĆ 1 DŁ. DACHU")]
    //    col127,
    //    [Display(Name = "POŁAĆ 1 DŁ.KRAW.")]
    //    col128,
    //    [Display(Name = "POŁAĆ 1 DŁ.GRZBIETU")]
    //    col129,
    //    [Display(Name = "POŁAĆ 1 KĄT NACH.")]
    //    col130,
    //    [Display(Name = "POŁAĆ 1 DŁUG.")]
    //    col131,
    //    [Display(Name = "POŁAĆ 1 SZER.")]
    //    col132,
    //    [Display(Name = "POŁAĆ 1 POWIERZCHNIA")]
    //    col133,
    //    [Display(Name = "POŁAĆ 1 POKRYCIE")]
    //    col134,
    //    [Display(Name = "POŁAĆ 1 AZYMUT")]
    //    col135,
    //    [Display(Name = "POŁAĆ 1 OKNA")]
    //    col136,
    //    [Display(Name = "POŁAĆ 1 ŚWIETLIKI")]
    //    col137,
    //    [Display(Name = "POŁAĆ 1 KOMINY")]
    //    col138,
    //    [Display(Name = "POŁAĆ 1 INSTALACJA POD")]
    //    col139,
    //    [Display(Name = "POŁAĆ 1 INST.ODGROM")]
    //    col140,
    //    [Display(Name = "POŁAĆ 2 TYP")]
    //    col141,
    //    [Display(Name = "POŁAĆ 2 WYS.BUD.")]
    //    col142,
    //    [Display(Name = "POŁAĆ 2 WYS.OKAPU")]
    //    col143,
    //    [Display(Name = "POŁAĆ 2 DŁ. DACHU")]
    //    col144,
    //    [Display(Name = "POŁAĆ 2 DŁ.KRAW.")]
    //    col145,
    //    [Display(Name = "POŁAĆ 2 DŁ.GRZBIETU")]
    //    col146,
    //    [Display(Name = "POŁAĆ 2 KĄT NACH.")]
    //    col147,
    //    [Display(Name = "POŁAĆ 2 DŁUG.")]
    //    col148,
    //    [Display(Name = "POŁAĆ 2 SZER.")]
    //    col149,
    //    [Display(Name = "POŁAĆ 2 POWIERZCHNIA")]
    //    col150,
    //    [Display(Name = "POŁAĆ 2 POKRYCIE")]
    //    col151,
    //    [Display(Name = "POŁAĆ 2 AZYMUT")]
    //    col152,
    //    [Display(Name = "POŁAĆ 2 OKNA")]
    //    col153,
    //    [Display(Name = "POŁAĆ 2 ŚWIETLIKI")]
    //    col154,
    //    [Display(Name = "POŁAĆ 2 KOMINY")]
    //    col155,
    //    [Display(Name = "POŁAĆ 2 INSTALACJA POD")]
    //    col156,
    //    [Display(Name = "POŁAĆ 2 INST.ODGROM")]
    //    col157,
    //    [Display(Name = "ELEW - WYS.")]
    //    col158,
    //    [Display(Name = "ELEW - SZER.")]
    //    col159,
    //    [Display(Name = "ELEW - AZYMUT")]
    //    col160,
    //    [Display(Name = "ELEW - POW.")]
    //    col161,
    //    [Display(Name = "ANULOWANA - KOMENTARZ")]
    //    col162,
    //    [Display(Name = "ANULOWANA - POWÓD")]
    //    col163,
    //    [Display(Name = "OST.ZM. - DATA")]
    //    col164,
    //    [Display(Name = "OST.ZM. - PRZEZ")]
    //    col165,
    //    //[Display(Name = "PRZYP. INSPEKTOR")]
    //    //col166,
    //    [Display(Name = "UWAGI")]
    //    col167,
    //    //[Display(Name = "ZAPLACONA")]
    //    //col168,
    //    [Display(Name = "Picture0")]
    //    col169,
    //    [Display(Name = "Picture1")]
    //    col170,
    //    [Display(Name = "Picture2")]
    //    col171,
    //    [Display(Name = "Picture3")]
    //    col172,
    //    [Display(Name = "Picture4")]
    //    col173,
    //    [Display(Name = "Picture5")]
    //    col174,
    //    [Display(Name = "Picture6")]
    //    col175,
    //    [Display(Name = "Picture7")]
    //    col176,
    //    [Display(Name = "Picture8")]
    //    col177,
    //    [Display(Name = "Picture9")]
    //    col178,
    //    [Display(Name = "Picture10")]
    //    col179,
    //    [Display(Name = "Picture11")]
    //    col180,
    //}
    //public class NeoExtractHelper
    //{
        
    //    public DataTable Data { get; private set; }
    //    private ICollection<Survey> source;
    //    private IFileStorage _fileStorage;
    //    public NeoExtractHelper(ICollection<Survey> surveys, IFileStorage FileStorage)
    //    {
    //        source = surveys;
    //        _fileStorage = FileStorage;
    //        Data = new DataTable();
    //        BuildHeader();
    //    }

    //    public static DataTable CreateDataTable<T>(IEnumerable<T> list)
    //    {
    //        Type type = typeof(T);
    //        var properties = type.GetProperties();

    //        DataTable dataTable = new DataTable();
    //        foreach (PropertyInfo info in properties)
    //        {
    //            dataTable.Columns.Add(new DataColumn(info.GetCustomAttribute(typeof(DisplayAttribute)).GetPropValue("Name"), Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
    //        }

    //        foreach (T entity in list)
    //        {
    //            object[] values = new object[properties.Length];
    //            for (int i = 0; i < properties.Length; i++)
    //            {
    //                values[i] = properties[i].GetValue(entity);
    //            }

    //            dataTable.Rows.Add(values);
    //        }

    //        return dataTable;
    //    }


    //    private void BuildHeader()
    //    {
    //        var cols = new List<DataColumn>();

    //        foreach (Enum item in Enum.GetValues(typeof(NeoExtractRecord)).Cast<NeoExtractRecord>())
    //        {
    //            DataColumn c = new DataColumn(item.DisplayName());
    //            switch (item.DisplayName())
    //            {
    //                case "OST.ZM. - DATA":
    //                    break;
    //                default:
    //                    break;
    //            }
    //            cols.Add(c);
    //        }
            
    //        Data.Columns.AddRange(cols.ToArray());

    //        //var header = Data.NewRow();

    //        //foreach (Enum item in Enum.GetValues(typeof(NeoExtractRecord)).Cast<NeoExtractRecord>())
    //        //{
    //        //    header.SetField<string>(item.DisplayName(), item.DisplayName());
    //        //}
    //        //Data.Rows.Add(header);
    //    }

    //    public void Convert()
    //    {
    //        foreach (var srv in source)
    //        {
    //            var row = Data.NewRow();

    //            row.SetField<string>("PRIORYTET",srv.Investment.PriorityIndex.ToString());
    //            row.SetField<string>("ID INWESTYCJI",srv.InvestmentId.ToString());
    //            row.SetField<string>("ID ANKIETY",srv.SurveyId.ToString());

    //            //SURVEY GENERAL
    //            row.SetField<string>("TYP OZE",srv.TypeFullDescription());
    //            row.SetField<string>("STATUS ANKIETY",srv.Status.DisplayName());

    //            ////INSPEKTOR
    //            //if (srv.Investment.InspectorId.HasValue &&
    //            //    srv.Investment.InspectorId != Guid.Empty)
    //            //{
    //            //    var usr = await _userManager.FindByIdAsync(srv.Investment.InspectorId.Value.ToString()));
    //            //    row.SetField<string>("PRZYP. INSPEKTOR",usr.LastName + " " + usr.FirstName;
    //            //}

    //            //INWESTYCJA
    //            row.SetField<string>("INWEST - ADRES - WOJ.",srv.Investment.Address.State.Text);
    //            row.SetField<string>("INWEST - ADRES - POW.",srv.Investment.Address.District.Text);
    //            row.SetField<string>("INWEST - ADRES - GM.",srv.Investment.Address.Commune.FullName);
    //            row.SetField<string>("INWEST - ADRES - KOD",srv.Investment.Address.PostalCode);
    //            row.SetField<string>("INWEST - ADRES - MIEJSC",srv.Investment.Address.City);
    //            row.SetField<string>("INWEST - ADRES - ULICA",srv.Investment.Address.Street);
    //            row.SetField<string>("INWEST - ADRES - NR BUD.",srv.Investment.Address.BuildingNo);
    //            row.SetField<string>("INWEST - ADRES - NR MIESZK",srv.Investment.Address.ApartmentNo);

    //            //WŁAŚCICIELE
    //            if (srv.Investment.InvestmentOwners != null)
    //            {
    //                for (int i = 0; i <= (srv.Investment.InvestmentOwners.Count - 1); i++)
    //                {
    //                    var owner = srv.Investment.InvestmentOwners.ElementAt(i).Owner;
    //                    row.SetField<string>("WŁAŚCICIEL " + i.ToString(),owner.PartnerName2 + " " + srv.Investment.InvestmentOwners.ElementAt(i).Owner.PartnerName1);
    //                    row.SetField<string>("WŁAŚCICIEL ADRES " + i.ToString(), owner.Address.SingleLine);
    //                    row.SetField<string>("WŁAŚCICIEL TEL " + i.ToString(), owner.PhoneNumber);
    //                    row.SetField<string>("WŁAŚCICIEL MAIL " + i.ToString(), owner.Email);
    //                }
    //            }

    //            //INWESTYCJA OGOLNE

    //            row.SetField<string>("RODZ. DZIAŁALN.",srv.Investment.BusinessActivity.DisplayName());
    //            row.SetField<string>("PALIWO GŁ.CO",srv.Investment.CentralHeatingFuel.DisplayName());
    //            row.SetField<string>("RODZAJ GŁ.CO",srv.Investment.CentralHeatingType != CentralHeatingType.Other ?
    //                                       srv.Investment.CentralHeatingType.DisplayName() :
    //                                       srv.Investment.CentralHeatingTypeOther);
    //            row.SetField<short>("ROK BUDOWY",srv.Investment.CompletionYear);
    //            row.SetField<string>("POW. OGRZEWANA",srv.Investment.HeatedArea.ToString());
    //            row.SetField<string>("PALIWO GŁ. CW",srv.Investment.HotWaterFuel.DisplayName());
    //            row.SetField<string>("RODZAJ GŁ. CW",srv.Investment.HotWaterType.DisplayName());
    //            row.SetField<string>("INTERNET W M.INW.",srv.Investment.InternetAvailable.AsYesNo());
    //            row.SetField<string>("NR KS. WIECZ.",srv.Investment.LandRegisterNo);
    //            row.SetField<string>("L.MIESZKAŃCÓW",srv.Investment.NumberOfOccupants.ToString());
    //            row.SetField<string>("NR DZIAŁKI",srv.Investment.PlotNumber);
    //            row.SetField<string>("STAN BUD.",srv.Investment.Stage.DisplayName());
    //            row.SetField<double>("POW. CAŁK.",srv.Investment.TotalArea);
    //            row.SetField<string>("RODZ.BUD.",srv.Investment.Type.DisplayName());
    //            row.SetField<double>("POW. UŻYTK.",srv.Investment.UsableArea);

    //            //AIRCOND
    //            if (srv.AirCondition != null)
    //            {
    //                row.SetField<string>("ISTN. KLIMAT.",srv.AirCondition.Exists.AsYesNo());
    //                row.SetField<string>("KLIMAT.PLANOWANA",srv.AirCondition.isPlanned.AsYesNo());
    //                row.SetField<string>("MECH.WENT.ISTN.",srv.AirCondition.MechVentilationExists.AsYesNo());
    //                row.SetField<string>("TYP INST.CHŁODZ.",srv.AirCondition.Type.DisplayName());
    //            }
    //            //ENERGY AUDIT
    //            if (srv.Audit != null)
    //            {
    //                row.SetField<string>("PAR.DOD.ŹR.CIEPŁA",srv.Audit.AdditionalHeatParams);
    //                row.SetField<string>("DOD.ŹR.CIEPŁA",srv.Audit.AdditionalHeatSource.AsYesNo());
    //                row.SetField<double>("ŚR.ROCZNE ZUŻYCIE CO",srv.Audit.AverageYearlyFuelConsumption);
    //                row.SetField<decimal>("ŚR.ROCZNE KOSZTY CO",srv.Audit.AverageYearlyHeatingCosts);
    //                row.SetField<string>("MAX TEMP. PIECA",srv.Audit.BoilerMaxTemp.ToString());
    //                row.SetField<string>("PIEC - MOC",srv.Audit.BoilerNominalPower.ToString());
    //                row.SetField<string>("PIEC - PLAN. WYM.",srv.Audit.BoilerPlannedReplacement.AsYesNo());
    //                row.SetField<string>("PIEC - ROK PROD.",srv.Audit.BoilerProductionYear.ToString());
    //                row.SetField<string>("CO PODLOG.",srv.Audit.CHFRadiantFloorInstalled.AsYesNo());
    //                row.SetField<string>("PLAN.POMPA BĘDZIE JEDYNYM ŹR.",srv.Audit.CHIsHPOnlySource.AsYesNo());
    //                row.SetField<string>("CO PODLOG. - PROCENT POW.",srv.Audit.CHRadiantFloorAreaPerc.ToString());
    //                row.SetField<string>("CO GRZEJNIKI",srv.Audit.CHRadiatorsInstalled.AsYesNo());
    //                row.SetField<string>("CO GRZEJNIKI - TYP",srv.Audit.CHRadiatorType.DisplayName());
    //                row.SetField<string>("UMOWA KOMPLEKS.",srv.Audit.ComplexAgreement.AsYesNo());
    //                row.SetField<decimal>("EE - ŚR. KOSZT/MC.",srv.Audit.ElectricityAvgMonthlyCost);
    //                row.SetField<double>("EE - MOC PRZYŁ.",srv.Audit.ElectricityPower);
    //                row.SetField<string>("EE - DOD. LICZNIK",srv.Audit.ENAdditionalConsMeter.AsYesNo());
    //                row.SetField<string>("EE - UZIEMIENIE",srv.Audit.ENIsGround.AsYesNo());
    //                row.SetField<string>("EE - PLANOWANA MOC",srv.Audit.ENPowerLevel.ToString());
    //                row.SetField<string>("CW - MOC",srv.Audit.HWSourcePower.ToString());
    //                row.SetField<string>("EE - L. FAZ",srv.Audit.PhaseCount.DisplayName());
    //                row.SetField<double>("EE - ROCZNE ZUŻYCIE",srv.Audit.PowerAvgYearlyConsumption);
    //                row.SetField<string>("EE - DYSTRYBUTOR",srv.Audit.PowerCompanyName.DisplayName());
    //                row.SetField<string>("EE - UMIEJSC. LICZNIKA",srv.Audit.PowerConsMeterLocation.DisplayName());
    //                row.SetField<string>("EE - RODZ. PRZYŁ.",srv.Audit.PowerSupplyType.DisplayName());
    //                row.SetField<double>("CW - POW. WĘŻ.",srv.Audit.TankCoilSize);
    //                row.SetField<string>("CW - ISTN. ZASOBNIK",srv.Audit.TankExists.AsYesNo());
    //                row.SetField<double>("CW - OBJ. ZASOBNIKA",srv.Audit.TankVolume);
    //            }

    //            //BATHROOM
    //            if (srv.BathRoom != null)
    //            {
    //                row.SetField<string>("ISTN. WANNA",srv.BathRoom.BathExsists.AsYesNo());
    //                row.SetField<double>("OBJ. WANNY",srv.BathRoom.BathVolume);
    //                row.SetField<string>("L.ŁAZIENEK",srv.BathRoom.NumberOfBathrooms.ToString());
    //                row.SetField<string>("ISTN.PRYSZNIC",srv.BathRoom.ShowerExists.AsYesNo());
    //            }

    //            //BOILERROOM
    //            if (srv.BoilerRoom != null)
    //            {
    //                row.SetField<string>("KOTŁOWNIA - ISTNIEJE",srv.BoilerRoom.RoomExists.AsYesNo());
    //                row.SetField<string>("KOTŁOWNIA - ISTN.WOLNY PRZEW.WENT",srv.BoilerRoom.AirVentilationExists.AsYesNo());
    //                row.SetField<string>("KOTŁOWNIA - SZER.DRZWI",srv.BoilerRoom.DoorHeight.ToString());
    //                row.SetField<double>("KOTŁOWNIA - WYS.",srv.BoilerRoom.Height);
    //                row.SetField<string>("KOTŁOWNIA - INST.400V",srv.BoilerRoom.HighVoltagePowerSupply.AsYesNo());
    //                row.SetField<string>("KOTŁOWNIA - ISTN. CYRKULACJA",srv.BoilerRoom.HWCirculationInstalled.AsYesNo());
    //                row.SetField<string>("KOTŁOWNIA - ISTN. INST. CW",srv.BoilerRoom.HWInstalled.AsYesNo());
    //                row.SetField<string>("KOTŁOWNIA - ISTN. REDUKTOR C.",srv.BoilerRoom.HWPressureReductorExists.AsYesNo());
    //                row.SetField<string>("KOTŁOWNIA - SUCHA I > 0 ST.",srv.BoilerRoom.IsDryAndWarm.AsYesNo());
    //                row.SetField<double>("KOTŁOWNIA - DŁUG.",srv.BoilerRoom.Length);
    //                row.SetField<string>("KOTŁOWNIA - ISTN. 3 UZIEM.GNIAZDA",srv.BoilerRoom.ThreePowerSuppliesExists.AsYesNo());
    //                row.SetField<double>("KOTŁOWNIA - KUBATURA",srv.BoilerRoom.Volume);
    //                row.SetField<double>("KOTŁOWNIA - SZER.",srv.BoilerRoom.Width);
    //            }

    //            //BUILDING
    //            if (srv.Building != null)
    //            {
    //                row.SetField<string>("IZOLACJA - GRUBOŚĆ",srv.Building.InsulationThickness.ToString());
    //                row.SetField<string>("IZOLACJA - RODZAJ",srv.Building.InsulationType == InsulationType.Ins_3 ?
    //                               srv.Building.InsulationTypeOther != null ?
    //                               srv.Building.InsulationTypeOther.ToString() : "" :
    //                               srv.Building.InsulationType.DisplayName());
    //                row.SetField<string>("TECHNOLOGIA WYKONANIA",srv.Building.TechnologyType.DisplayName());
    //                row.SetField<double>("KUBATURA BUD.",srv.Building.Volume);
    //                row.SetField<string>("MATERIAŁ ŚCIAN",srv.Building.WallMaterialOther != null ? srv.Building.WallMaterialOther.ToString() : "");
    //                row.SetField<string>("GRUBOŚĆ ŚCIAN",srv.Building.WallThickness.ToString());
    //            }

    //            //GENERAL
    //            row.SetField<string>("ANULOWANA - KOMENTARZ",srv.CancelComments != null ? srv.CancelComments.ToString() : "");
    //            row.SetField<string>("ANULOWANA - POWÓD",srv.CancelType.HasValue ? srv.CancelType.DisplayName() : "");
    //            row.SetField<DateTime>("OST.ZM. - DATA",srv.ChangedAt.ToLocalTime());
    //            //if (srv.ChangedBy != Guid.Empty)
    //            //{
    //            //    var usr = await _userManager.FindByIdAsync(srv.ChangedBy.ToString());
    //            //    row.SetField<string>("OST.ZM. - PRZEZ",usr.LastName + " " + usr.FirstName;
    //            //}

    //            row.SetField<string>("UWAGI",srv.FreeCommments != null ? srv.FreeCommments.ToString() : "");
    //            //row.SetField<string>("ZAPLACONA",srv.IsPaid.AsYesNo());

    //            //GROUND
    //            if (srv.Ground != null)
    //            {
    //                row.SetField<double>("GRUNT - POW.",srv.Ground.Area);
    //                row.SetField<string>("GRUNT - BYLY TEREN WOJSK",srv.Ground.FormerMilitary.AsYesNo());
    //                row.SetField<string>("GRUNT - ISTN.INSTALACJA",srv.Ground.OtherInstallation.AsYesNo());
    //                row.SetField<string>("GRUNT - INSTALACJA TYP",srv.Ground.OtherInstallationType != null ? srv.Ground.OtherInstallationType : "");
    //                row.SetField<string>("GRUNT - GRUZ,SKAŁY",srv.Ground.Rocks.AsYesNo());
    //                row.SetField<string>("GRUNT - NACHYLENIE",srv.Ground.SlopeTerrain.DisplayName());
    //                row.SetField<string>("GRUNT - PODMOKŁY",srv.Ground.WetLand.AsYesNo());
    //            }

    //            // PLANNED INSTALLATION
    //            if (srv.PlannedInstall != null)
    //            {
    //                row.SetField<string>("ZESTAW KLIENTA",srv.PlannedInstall.Configuration.DisplayName());
    //                row.SetField<string>("LOKALIZACJA INSTALACJI",srv.PlannedInstall.Localization.DisplayName());
    //                row.SetField<string>("INSTALACJA NA SCIANIE",srv.PlannedInstall.OnWallPlacementAvailable.AsYesNo());
    //                row.SetField<string>("PRZEZN. BUDYNKU",srv.PlannedInstall.Purpose.DisplayName());
    //            }

    //            //ROOF
    //            if (srv.RoofPlanes != null)
    //            {
    //                int i = 0;
    //                foreach (var roof in srv.RoofPlanes)
    //                {
    //                    i++;
    //                    row.SetField<string>("POŁAĆ " + i.ToString() + " TYP",roof.Type.DisplayName());
    //                    row.SetField<double>("POŁAĆ " + i.ToString() + " WYS.BUD.",roof.BuildingHeight);
    //                    row.SetField<string>("POŁAĆ " + i.ToString() + " KOMINY",roof.Chimney.AsYesNo());
    //                    row.SetField<double>("POŁAĆ " + i.ToString() + " DŁ.KRAW.",roof.EdgeLength);
    //                    row.SetField<string>("POŁAĆ " + i.ToString() + " INSTALACJA POD",roof.InstallationUnderPlane.AsYesNo());
    //                    row.SetField<double>("POŁAĆ " + i.ToString() + " DŁUG.",roof.Width);
    //                    row.SetField<double>("POŁAĆ " + i.ToString() + " SZER.",roof.Length);
    //                    row.SetField<string>("POŁAĆ " + i.ToString() + " INST.ODGROM",roof.LightingProtection.AsYesNo());
    //                    row.SetField<double>("POŁAĆ " + i.ToString() + " WYS.OKAPU",roof.OkapHeight);
    //                    row.SetField<double>("POŁAĆ " + i.ToString() + " DŁ.GRZBIETU",roof.RidgeWeight);
    //                    row.SetField<double>("POŁAĆ " + i.ToString() + " DŁ. DACHU",roof.RoofLength);
    //                    row.SetField<string>("POŁAĆ " + i.ToString() + " POKRYCIE",roof.RoofMaterial.DisplayName());
    //                    row.SetField<string>("POŁAĆ " + i.ToString() + " ŚWIETLIKI",roof.SkyLights.AsYesNo());
    //                    row.SetField<double>("POŁAĆ " + i.ToString() + " KĄT NACH.",roof.SlopeAngle);
    //                    row.SetField<double>("POŁAĆ " + i.ToString() + " POWIERZCHNIA",roof.SurfaceArea);
    //                    row.SetField<double>("POŁAĆ " + i.ToString() + " AZYMUT",roof.SurfaceAzimuth);
    //                    row.SetField<string>("POŁAĆ " + i.ToString() + " OKNA",roof.Windows.AsYesNo());                        
    //                }
    //            }

    //            //WALL
    //            if (srv.Wall != null)
    //            {
    //                row.SetField<double>("ELEW - AZYMUT",srv.Wall.Azimuth);
    //                row.SetField<double>("ELEW - WYS.", srv.Wall.Height);
    //                row.SetField<double>("ELEW - SZER.", srv.Wall.Width);
    //                row.SetField<double>("ELEW - POW.",srv.Wall.UsableArea);
    //            }

    //            //ZDJECIA
    //            foreach (var item in srv.Photos(_fileStorage))
    //            {
    //                row.SetField<Uri>(item.Key, new Uri(item.Value.ToString()));
    //            }

    //            Data.Rows.Add(row);
    //        }

    //    }

    //}
}
