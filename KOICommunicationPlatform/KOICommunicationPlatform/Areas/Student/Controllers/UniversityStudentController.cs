using Microsoft.AspNetCore.Mvc;

namespace KOICommunicationPlatform.Areas.Student.Controllers
{
    public class UniversityStudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
