using KOICommunicationPlatform.Areas.Admin.Controllers;
using KOICommunicationPlatform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KOICommunicationPlatform.Areas.Client.Controllers
{
    [Area("Client")]
    public class ClientController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClientController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var user = _userManager.FindByIdAsync(userId)?.Result;
            // Check if the user is not null and has a valid name
            if (user != null)
            {
                // Pass the user's name to the view
                ViewBag.Organization = user.Organization; // You can use any property like user.FirstName, user.LastName, etc.
            }

            return View();
        }
    }
}
