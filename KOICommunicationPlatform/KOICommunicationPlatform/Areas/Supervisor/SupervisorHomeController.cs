using Microsoft.AspNetCore.Mvc;

namespace KOICommunicationPlatform.Areas.Supervisor
{
    [Area("Supervisor")]
    public class SupervisorHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
