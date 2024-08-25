using Microsoft.AspNetCore.Mvc;

namespace KOICommunicationPlatform.Areas.Student.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
