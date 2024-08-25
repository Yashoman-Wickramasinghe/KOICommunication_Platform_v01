using Microsoft.AspNetCore.Mvc;

namespace KOICommunicationPlatform.Areas.Student.Controllers
{
    [Area("Student")]
    public class StudentHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
