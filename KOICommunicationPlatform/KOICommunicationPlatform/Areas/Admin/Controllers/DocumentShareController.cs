using Microsoft.AspNetCore.Mvc;

namespace KOICommunicationPlatform.Areas.Admin.Controllers
{
    [Area("Admin")]
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
