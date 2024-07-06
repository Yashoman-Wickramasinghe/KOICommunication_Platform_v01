using Microsoft.AspNetCore.Mvc;

namespace KOICommunicationPlatform.Controllers
{
    public class DocumentShareController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UploadDocuments()
        {//test
            return View();
        }
    }
}
