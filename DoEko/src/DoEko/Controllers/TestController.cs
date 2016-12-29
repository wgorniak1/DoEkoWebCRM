using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DoEko.Models.DoEko;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DoEko.Models.Identity;
using DoEko.Models.DoEko.Addresses;
using DoEko.Controllers.Helpers;
using DoEko.Models.DoEko.Survey;
using DoEko.ViewModels.SurveyViewModels;
using DoEko.ViewModels.TestViewModels;
using DoEko.Services;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DoEko.Controllers
{
    public class TestController : Controller
    {
        private DoEkoContext _context;
        private UserManager<ApplicationUser> _userManager;
        public TestController(DoEkoContext context, UserManager<ApplicationUser> userManager, IFileStorage fileStorage)
        {
            _context = context;
            _userManager = userManager;
            _fileStorage = fileStorage;
        }

        private IFileStorage _fileStorage;

        // GET: Test
        public ActionResult Index()
        {
            return View();
        }

        // GET: Test/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Test/Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ListPhotos()
        {
            //disable change tracking
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            IList<ListPhotosViewModel> model = new List<ListPhotosViewModel>();
            ListPhotosViewModel modelItem;

            //read photos from azurestorage
            CloudBlobContainer ContainerSrv = _fileStorage.GetBlobContainer(enuAzureStorageContainerType.Survey);
            CloudBlobContainer ContainerInv = _fileStorage.GetBlobContainer(enuAzureStorageContainerType.Investment);
            var SurveyBlockBlobs = ContainerSrv.ListBlobs(useFlatBlobListing: true).OfType<CloudBlockBlob>();
            var InvestBlockBlobs = ContainerInv.ListBlobs(useFlatBlobListing: true).OfType<CloudBlockBlob>();

            //collect additional information
            foreach (var BlockBlob in SurveyBlockBlobs)
            {
                var partNames = BlockBlob.Name.Split('/').Reverse().ToArray();
                if (partNames.Length < 3)
                {
                    continue;
                }
                var srvId = Guid.Parse(partNames[2]);
                if (srvId == Guid.Empty)
                {
                    continue;
                }
                //2= guid / 1 = pictureid / 0 = filename
                try
                {
                    modelItem = model.Single(m => m.Survey.SurveyId == srvId);
                }
                catch (Exception)
                {
                    modelItem = new ListPhotosViewModel(GetSurvey(srvId));
                    model.Add(modelItem);
                }

                modelItem.Attachments.Add(partNames[1], new SurveyAttachment()
                {
                    FileName = partNames[0],
                    PictureId = partNames[1],
                    Url = BlockBlob.Uri.ToString()
                });
            };

            SurveyBlockBlobs = null;
            SurveyBlockBlobs = null;
            ContainerSrv = null;

            foreach (var BlockBlob in InvestBlockBlobs)
            {
                //2= guid / 1 = pictureid / 0 = filename
                var partNames = BlockBlob.Name.Split('/').Reverse().ToArray();
                if (partNames.Length < 3)
                {
                    continue;
                }

                var invId = Guid.Parse(partNames[2]);
                var srvIds = _context.Surveys.Where(s => s.InvestmentId == invId).Select(s=>s.SurveyId).ToArray();
                foreach (var srvId in srvIds)
                {
                    if (srvId == Guid.Empty)
                    {
                        continue;
                    }
                    try
                    {
                        modelItem = model.Single(m => m.Survey.SurveyId == srvId);
                    }
                    catch (Exception)
                    {
                        modelItem = new ListPhotosViewModel(GetSurvey(srvId));
                        model.Add(modelItem);
                    }
                    modelItem.Attachments.Add(partNames[1],new SurveyAttachment()
                    {
                        FileName = partNames[0],
                        PictureId = partNames[1],
                        Url = BlockBlob.Uri.ToString()
                    });
                }
            };
            //return view
            model
                .OrderBy(m => m.Survey.Investment.Contract.ShortDescription)
                .ThenBy(m => m.Survey.Investment.Address.SingleLine)
                .ThenBy(m => m.Survey.Type);
            TempData["model"] = model;
            return View(model);
        }
        [HttpGet]
        public async Task<ActionResult> PhotosAdjust()
        {
            CloudBlobContainer ContainerSrv = _fileStorage.GetBlobContainer(enuAzureStorageContainerType.Survey);
            CloudBlobContainer ContainerInv = _fileStorage.GetBlobContainer(enuAzureStorageContainerType.Investment);
            var SurveyBlockBlobs = ContainerSrv.ListBlobs(useFlatBlobListing: true).OfType<CloudBlockBlob>();

            foreach (var BlockBlob in SurveyBlockBlobs)
            {
                var partNames = BlockBlob.Name.Split('/').Reverse().ToArray();

                if (partNames[1].Equals("Picture0") || partNames[1].Equals("Picture5"))
                {
                    //calculate investment
                    Guid srvid = Guid.Parse(partNames[2]);
                    Guid invid = await _context.Surveys.Where(s => s.SurveyId == srvid).Select(s => s.InvestmentId).SingleAsync();
                    if (invid != Guid.Empty)
                    {
                        partNames[2] = invid.ToString();

                        string targetName = partNames[2] + '/' + partNames[1] + '/' + partNames[0];

                        CloudBlockBlob targetBlob = ContainerInv.GetBlockBlobReference(targetName);
                        targetBlob.StartCopy(BlockBlob);
                        while (targetBlob.CopyState.Status != CopyStatus.Success)
                        {
                            //
                        }
                        BlockBlob.Delete();
                    }
                }
            };

            return Json("ok");
        }

        // POST: Test/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));
            try
            {
                // TODO: Add insert logic here
                test model = new Models.DoEko.test { checkme = true, PaymentId = Guid.NewGuid() };
                _context.Add(model);
                int result = await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch(Exception exc)
            {
                //exc.Message
                return Json(exc.Message);
            }
        }

        // GET: Test/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Test/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Test/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Test/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult csvTest()
        {
            _context.CurrentUserId = Guid.Parse(_userManager.GetUserId(User));
            string line = "1; 98,40 z³ ; 80,00 z³ ;Kocio³ na Pellet;;;Lubelskie;Janowski;Chrzanów (Gmina W.);37-500; Tuczempy ;Mickiewicza;44;;Ryszard;Trelka;Podlaskie;Bia³ostocki;Choroszcz (Gmina M-W);37-500;Tuczempy;Mickiewicza;3A;1;266;30621;661 111 760;;;";

            InvestmentUploadHelper uploadhelper = new InvestmentUploadHelper(_context);
            uploadhelper.ContractId = _context.Contracts.First().ContractId;

            try
            {   
                //set line to parse
                uploadhelper.Record = line;


                Address InvestmentAddress = uploadhelper.ParseInvestmentAddress();
                Investment Investment = uploadhelper.ParseInvestment();
                Investment.Address = InvestmentAddress;

                Address OwnerAddress = uploadhelper.ParseOwnerAddress();
                BusinessPartnerPerson Owner = (BusinessPartnerPerson) uploadhelper.ParseInvestmentOwner();

                if (OwnerAddress.SingleLine == InvestmentAddress.SingleLine)
                {
                    Owner.Address = InvestmentAddress;
                    OwnerAddress = null;
                }
                else
                {
                    Owner.Address = OwnerAddress;
                }

                InvestmentOwner InvestmentOwner = new InvestmentOwner
                {
                    Investment = Investment,
                    InvestmentId = Investment.InvestmentId,
                    Owner = Owner,
                    OwnerId = Owner.BusinessPartnerId
                };

                ICollection<Survey> Surveys = uploadhelper.ParseSurveys();
                foreach (var Survey in Surveys)
                {
                    Survey.Investment = Investment;
                    Survey.InvestmentId = Investment.InvestmentId;
                };
                
            }
            catch (Exception exc)
            {
                return Json(exc.Message);
            }

            return Json("ok");
        }

        public async Task<ActionResult> TestOwner()
        {
            SurveyCentralHeating survey = await _context.SurveysCH
                .Include(s=>s.Investment).ThenInclude(i=>i.Address).ThenInclude(a=>a.Commune)
                .Include(s=>s.Investment).ThenInclude(i=>i.InvestmentOwners).ThenInclude(io=>io.Owner).ThenInclude(o=>o.Address)
                .FirstAsync();

            DetailsCHViewModel model = new DetailsCHViewModel(survey);

            return View(model);
        }

        [HttpGet]
        public IActionResult Test(int Id)
        {
            TestViewModel model = new TestViewModel();

            if (Id==1)
            {
                model.checkbox = true;
            }
            

            return View(model);
        }

        [HttpPost]
        public IActionResult Test(TestViewModel model)
        {
            return Ok(new { checkbox = model.checkbox, stringchk = model.checkbox.ToString() });
        }

        public IActionResult TestDouble()
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            Survey srv = _context.Surveys
                .Include(s=>s.BoilerRoom)
                .Single(s => s.SurveyId == Guid.Parse("386c6b9a-fb0c-4aa1-a7b7-00e751deb8b6"));

            return View(srv.BoilerRoom);
        }

        private Survey GetSurvey(Guid id)
        {

            SurveyType type = _context.Surveys.Where(s => s.SurveyId == id).Select(s => s.Type).First();

            switch (type)
            {
                case SurveyType.CentralHeating:
                    return _context.SurveysCH
                        .Select(s => new SurveyCentralHeating {
                            SurveyId = s.SurveyId,
                            Type = s.Type,
                            RSEType = s.RSEType,
                            Investment = new Investment {
                                Address = s.Investment.Address,
                                Contract = new Contract {
                                    ShortDescription = s.Investment.Contract.ShortDescription
                                }
                            }
                        })
                        .Single(s => s.SurveyId == id);
                case SurveyType.HotWater:
                    return _context.SurveysHW
                        .Select(s => new SurveyHotWater
                        {
                            SurveyId = s.SurveyId,
                            Type = s.Type,
                            RSEType = s.RSEType,
                            Investment = new Investment
                            {
                                Address = s.Investment.Address,
                                Contract = new Contract
                                {
                                    ShortDescription = s.Investment.Contract.ShortDescription
                                }
                            }
                        }).Single(s => s.SurveyId == id);
                case SurveyType.Energy:
                    return _context.SurveysEN
                        .Select(s => new SurveyEnergy
                        {
                            SurveyId = s.SurveyId,
                            Type = s.Type,
                            RSEType = s.RSEType,
                            Investment = new Investment
                            {
                                Address = s.Investment.Address,
                                Contract = new Contract
                                {
                                    ShortDescription = s.Investment.Contract.ShortDescription
                                }
                            }
                        })
                        .Single(s => s.SurveyId == id);
                default:
                    return null;
            }
        }
    }
}