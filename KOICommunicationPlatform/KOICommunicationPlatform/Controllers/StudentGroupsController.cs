using Microsoft.AspNetCore.Mvc;

namespace KOICommunicationPlatform.Controllers
{
    public class StudentGroupsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ViewStudentGroups()
        {
            return View();
        }
    }
}
