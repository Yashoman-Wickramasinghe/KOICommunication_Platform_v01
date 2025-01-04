using KOICommunicationPlatform.Areas.Admin.Controllers;
using KOICommunicationPlatform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KOICommunicationPlatform.Areas.Client.Controllers
{
    [Area("Client")]
    public class ClientController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public ClientController(IUnitOfWork unitOfWork,ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var user = _userManager.FindByIdAsync(userId)?.Result;

            ViewBag.ClientId = userId;
            // Check if the user is not null and has a valid name
            if (user != null)
            {
                // Pass the user's name to the view
                ViewBag.Organization = user.Organization; 

                bool isexistCotractYesNo = _unitOfWork.ApplicationUser.IsExist(x=>x.SubmissionLink != null);

                var subLinkDocumentContract = _unitOfWork.ApplicationUser.GetFirstOrDefault(x=>x.SubmissionLink != null && x.Id == userId);

                Guid userIdGuid = Guid.Parse(userId);

                // Step 1: Retrieve StudentGroupHD records for the given ClientId
                var studentGroupHDs = _unitOfWork.StudentGroupHD
                    .GetAll(x => x.ClientId == userIdGuid)
                    .ToList();

                // Step 2: Retrieve StudentGroupDetail records with included Student entity
                var studentGroupDetails = _unitOfWork.StudentGroupDetail
                    .GetAll(
                        x => studentGroupHDs.Select(g => g.Id).Contains(x.StudentGroupHD.Id),
                        includeProperties: "Student" // Eager load the Student entity
                    )
                    .ToList();

                // Step 3: Retrieve the students associated with the StudentGroupDetail records
                var studentList = studentGroupDetails
                    .Where(d => d.Student != null) // Filter out null Students
                    .Select(d => d.Student)
                    .Distinct() // Ensure unique students
                    .ToList();

                ViewBag.StudentList = studentGroupDetails;
 
                // Set the ViewBag property
                ViewBag.DevGroupCount = studentGroupHDs.Count();


                ViewBag.DevGroupCount = studentGroupHDs.Count();

                if (isexistCotractYesNo == true && subLinkDocumentContract != null)
                {
                    if (isexistCotractYesNo == true)
                    {
                        ViewBag.isexistCotractYesNo = "Yes";
                    }

                  
                    var files = Directory.GetFiles(subLinkDocumentContract.SubmissionLink);
                    ViewBag.files = files;
                    ViewBag.fileNames = files.Select(Path.GetFileName).ToList();
                    ViewBag.subLinkDocumentContract = subLinkDocumentContract.SubmissionLink;
                }
                else {
                    ViewBag.isexistCotractYesNo = "No";
                }
            }
            return View();
        }

        public IActionResult DownloadFile(string clientId, string fileName)
        {
            var client = _unitOfWork.ApplicationUser.GetAll().FirstOrDefault(c => c.Id == clientId);

            if (client == null || string.IsNullOrEmpty(client.SubmissionLink))
            {
                return NotFound();
            }

            var filePath = Path.Combine(client.SubmissionLink, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var contentType = "application/octet-stream";

            return File(fileBytes, contentType, fileName);
        }

    }
}
