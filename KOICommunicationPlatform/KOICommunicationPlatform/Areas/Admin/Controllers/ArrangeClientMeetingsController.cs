using Microsoft.AspNetCore.Mvc;

namespace KOICommunicationPlatform.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArrangeClientMeetingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewClientMeeting()
        {
            return View();
        }
    }
}
