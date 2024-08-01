using Microsoft.AspNetCore.Mvc;
namespace KOICommunicationPlatform.Areas.Admin.Controllers
{
    [Area("Admin")]
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
