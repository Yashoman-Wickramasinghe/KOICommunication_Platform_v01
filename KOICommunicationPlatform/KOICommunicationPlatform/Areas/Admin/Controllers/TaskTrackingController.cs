using Microsoft.AspNetCore.Mvc;

namespace KOICommunicationPlatform.Controllers
{
    public class TaskTrackingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
