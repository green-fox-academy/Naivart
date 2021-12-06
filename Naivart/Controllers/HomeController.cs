using Microsoft.AspNetCore.Mvc;
using Naivart.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Controllers
{
    public class HomeController : Controller
    {
        public Service Service { get; set; }
        public HomeController(Service service)
        {
            Service = service;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
