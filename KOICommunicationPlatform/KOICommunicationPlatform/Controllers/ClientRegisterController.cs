using Microsoft.AspNetCore.Mvc;

namespace KOICommunicationPlatform.Controllers
{
    public class ClientRegisterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewClient ()
        {
            return View();
        }

        public IActionResult ArrangeClientMeeting()
        {
            return View();
        }
        public IActionResult ViewClientMeeting()
        {
            return View();
        }
    }
}
