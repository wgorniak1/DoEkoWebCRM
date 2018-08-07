using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DoEko.Services;
using Microsoft.Extensions.Logging;
using DoEko.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DoEko.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/v1/Files")]
    public class ApiFilesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly IFileStorage _fileStorage;

        public ApiFilesController(
            UserManager<ApplicationUser> userManager,
            ILoggerFactory loggerFactory,
            IFileStorage fileStorage )
        {
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _fileStorage = fileStorage;
        }

        [HttpGet]
        [Route("Templates")]
        public async Task<IActionResult> GetTemplate([FromQuery] string Type)
        {
            var container = await _fileStorage.GetBlobContainerAsync(EnuAzureStorageContainerType.Templates);
            var files = await container.ListBlobsAsync(Type, true, BlobListingDetails.None, null, new BlobContinuationToken(), null, null);
            
            if (files.Count() > 0)
            {
                return Ok(new { url = files.First().Uri.AbsoluteUri });
            }
            else
            {
                return NotFound();
            }
        }

    }
}