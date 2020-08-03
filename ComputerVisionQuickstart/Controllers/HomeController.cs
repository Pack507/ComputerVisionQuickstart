using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ComputerVisionQuickstart.Models;
using Newtonsoft.Json;

namespace ComputerVisionQuickstart.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            Customer newObj = new Customer();
            newObj.Identification = "8-877-2330";
            newObj.FullName = "Noemi Gonzalez";
            newObj.Age = 27;

            ViewBag.DatosNoemi = newObj;

            Customer nuevoObj = new Customer();
            nuevoObj.Identification = "9-728-1643";
            nuevoObj.FullName = "Jose de Gonzalez mio";
            nuevoObj.Age = 32;

            ViewBag.DatosJose = nuevoObj;

            return View();
        }

        public IActionResult Privacy()
        {          
            if (TempData["LoadData"] != null)
            {
                //ViewData["LoadData"] = TempData["LoadData"];
                ViewData["LoadData"] = JsonConvert.DeserializeObject<List<ResponseModel>>(TempData["LoadData"].ToString());
            }

            return View();
        }

        public IActionResult Prototype()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
