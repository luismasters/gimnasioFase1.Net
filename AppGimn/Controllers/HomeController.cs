using System.Diagnostics;
using AppGimn.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppGimn.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // 1. Landing Page Pública del Software SaaS AppGimn
        public IActionResult Index()
        {
            return View();
        }

        // 2. Demo Interactiva del Gimnasio Ficticio "Aura Fitness Club"
        public IActionResult Demo()
        {
            return View();
        }

        public IActionResult Privacy()
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
