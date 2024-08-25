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
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {

            // Retrieve the user ID from the current context
            var userId = _userManager.GetUserId(User);

            // Find the user asynchronously
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                // Store the user in ViewData for use in the view
                ViewData["CurrentUser"] = user;

                // Determine the role to assign based on UserType
                string roleToAssign = user.UserType switch
                {
                    SD.Website_Admin => SD.Website_Admin,
                    SD.Website_Client => SD.Website_Client,
                    SD.Website_Student => SD.Website_Student,
                    _ => null
                };

                if (!string.IsNullOrEmpty(roleToAssign))
                {
                    // Ensure the role exists
                    if (!await _roleManager.RoleExistsAsync(roleToAssign))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(roleToAssign));
                    }

                    // Get all roles assigned to the user
                    var userRoles = await _userManager.GetRolesAsync(user);

                    // Remove all existing roles
                    if (userRoles.Any())
                    {
                        await _userManager.RemoveFromRolesAsync(user, userRoles);
                    }

                    // Assign the correct role
                    await _userManager.AddToRoleAsync(user, roleToAssign);
                }

                // Redirect based on the assigned role
                if (roleToAssign == SD.Website_Admin)
                {
                    return View();
                }
                if (roleToAssign == SD.Website_Client)
                {
                    return RedirectToAction("Index", "Client", new { area = "Client" });
                }
                if (roleToAssign == SD.Website_Student)
                {
                    return RedirectToAction("Index", "StudentHome", new { area = "UniversityStudent" });
                }
            }

            // Generate the URL for the login page
            string loginUrl = Url.Page("/Account/Login", new { area = "Identity" });

            // Redirect to the login page
            return Redirect(loginUrl);

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
