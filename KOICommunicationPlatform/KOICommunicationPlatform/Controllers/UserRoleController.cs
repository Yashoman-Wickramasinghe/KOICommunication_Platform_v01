using Microsoft.AspNetCore.Mvc;

namespace KOICommunicationPlatform.Controllers
{
    public class UserRoleController : Controller
    {
        //Define User Roles
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AssignUser()
        {
            return View();
        }
    }
}
