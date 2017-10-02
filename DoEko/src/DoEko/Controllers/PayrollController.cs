using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DoEko.Controllers
{
    public class PayrollController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}