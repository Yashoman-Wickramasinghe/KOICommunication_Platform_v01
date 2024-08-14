using KOICommunicationPlatform.Models;
using KOICommunicationPlatform.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;
using System.Diagnostics;

namespace KOICommunicationPlatform.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.Website_Admin},{SD.Website_Client},{SD.Website_Student}")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User); 
            var user = _userManager.FindByIdAsync(userId)?.Result;
            if (user != null)
            {
                if (user.UserType == SD.Website_Admin)
                {
                    return View();
                }
                else if (user.UserType == SD.Website_Client)
                {
                    return RedirectToAction("Index", "Client", new { area = "Client" });
                }
            }
           
            return View("AccessDenied");

            
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
