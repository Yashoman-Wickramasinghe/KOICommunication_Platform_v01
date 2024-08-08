using Microsoft.AspNetCore.Mvc;

namespace KOICommunicationPlatform.Areas.Admin.Controllers
{
    public class ClientMeetingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
