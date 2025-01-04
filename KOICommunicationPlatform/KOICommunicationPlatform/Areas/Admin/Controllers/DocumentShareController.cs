using KOICommunicationPlatform.Models;
using KOICommunicationPlatform.Models.ViewModels;
using KOICommunicationPlatform.Utilities.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;

namespace KOICommunicationPlatform.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DocumentShareController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IRegistrationEmailSender _registrationEmailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public DocumentShareController(IUnitOfWork unitOfWork,
            IWebHostEnvironment hostEnvironment,
            IRegistrationEmailSender registrationEmailSender,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
            _registrationEmailSender = registrationEmailSender;
            _userManager = userManager;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var tutorials = _unitOfWork.Tutorial.GetAll(includeProperties: "Subject,Subject.Course").Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = $"{(t.Subject?.Course?.CourseName ?? "N/A")} / {(t.Trimester ?? "N/A")} / {(t.Subject?.SubjectName ?? "N/A")} / {(t.Lab ?? "N/A")} / {(t.TutorialNo ?? "N/A")} / {(t.Day ?? "N/A")} / {(t.FromTime ?? "N/A")} / {(t.ToTime ?? "N/A")}"
            }).ToList();

            ViewBag.TutorialsDropdown = tutorials;

            return View();
        }

        [HttpGet]
        public JsonResult GetGroupsByTutorial(int tutorialId)
        {
            var groups = _unitOfWork.StudentGroupHD.GetAll(g => g.TutorialId == tutorialId)
                .Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.GroupGenerateId
                })
                .ToList();

            return Json(groups);
        }

        [HttpPost]
        public async Task<IActionResult> ViewDocUpload(int GroupId, int TutorialId)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            // Get the StudentGroupHD based on GroupId and TutorialId
            var studentHdId = _unitOfWork.StudentGroupHD.GetFirstOrDefault(x => x.TutorialId == TutorialId && x.Id == GroupId);
            if (studentHdId == null)
            {
                ModelState.AddModelError(string.Empty, "Student Group not found.");
                return PartialView("_ProjectTimelinePartial", new ProjectTimelineViewModel()); // Return empty view model on error
            }

            var studentGroupDetail = _unitOfWork.StudentGroupDetail
                .GetFirstOrDefault(x => x.StudentGroupHD.Id == studentHdId.Id);

            if (studentGroupDetail == null)
            {
                ModelState.AddModelError(string.Empty, "Student Group Detail not found.");
                return PartialView("_ProjectTimelinePartial", new ProjectTimelineViewModel()); // Return empty view model on error
            }

            // Fetch active deliverables
            var deliverables = _unitOfWork.ProjectDeliverable.GetAll(d => d.IsActive).ToList();

            // Fetch documents associated with the selected student group
            var documents = _unitOfWork.DocumentUpload
                .GetAll(d => d.IsActive && d.StudentGroupHDId == studentGroupDetail.StudentGroupHD.Id)
                .ToList();

            // Return the filtered deliverables and documents in the view model
            var viewModel = new ProjectTimelineViewModel
            {
                Deliverables = deliverables,
                Documents = documents
            };

            return PartialView("PartialDocumentUpload", viewModel);
        }

        public IActionResult DownloadFile(string filePath, string fileName)
        {
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var contentType = "application/octet-stream"; // Default MIME type for binary files
            return File(fileBytes, contentType, fileName);
        }

        [HttpPost]
        public IActionResult DeleteDocument(int documentId, string filePath)
        {
            // Fetch the document from the database
            var document = _unitOfWork.DocumentUpload.GetFirstOrDefault(d => d.Id == documentId);

            var comments = _unitOfWork.CommentsOnDocumentUpload.GetAll(x => x.DocumentId == documentId);

            if (document == null)
            {
                return Json(new { success = false, message = "Document not found." });
            }

            // Delete the file from the file system
            var fullPath = Path.Combine(_hostEnvironment.WebRootPath, filePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            // Delete the document record from the database
            _unitOfWork.DocumentUpload.Remove(document);
            _unitOfWork.Save();

            _unitOfWork.CommentsOnDocumentUpload.RemoveRange(comments);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Document deleted successfully." });
        }

        [HttpGet]
        public IActionResult GetComments(int documentId)
        {
            var comments = _unitOfWork.CommentsOnDocumentUpload
                .GetAll(c => c.DocumentId == documentId && c.IsActive)
                .Select(c => new
                {
                    c.Id,
                    c.Comment,
                    c.CreatedBy,
                    CreatedDateTime = c.CreatedDateTime.ToString("yyyy-MM-dd HH:mm")
                })
                .OrderByDescending(c => c.CreatedDateTime)
                .ToList();

            return Json(comments);
        }

        // Add a new comment
        [HttpPost]
        public IActionResult AddComment(int documentId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("Comment cannot be empty.");
            }

            var comment = new CommentsOnDocumentUpload
            {
                DocumentId = documentId,
                Comment = content,
                CreatedBy = User.Identity.Name, 
                CreatedDateTime = DateTime.Now,
                IsActive = true
            };

            _unitOfWork.CommentsOnDocumentUpload.Add(comment);
            _unitOfWork.CommentsOnDocumentUpload.Save();

            return Ok();
        }


        private string GetFirstLetter(string sentence)
        {
            if (string.IsNullOrWhiteSpace(sentence))
            {
                return string.Empty;
            }

            return sentence.Trim()[0].ToString().ToUpper();
        }

        private string GetCoursePrefix(int deliverableId)
        {
            // Retrieve the deliverable to get the courseId
            var deliverable = _unitOfWork.ProjectDeliverable.GetFirstOrDefault(d => d.Id == deliverableId);
            if (deliverable != null)
            {
                var course = _unitOfWork.Course.GetFirstOrDefault(x => x.Id == deliverable.CourseId);
                return course != null ? GetFirstLetter(course.CourseName) : "Unknown";
            }
            return "Unknown";
        }

        private string GetGroupFolderName(string studentId)
        {
            // Retrieve the student group detail to get the groupId
            var studentGroup = _unitOfWork.StudentGroupDetail.GetFirstOrDefault(x => x.Student.StudentId == studentId);
            return studentGroup != null ? studentGroup.GroupGenerateId : "UnknownGroup";
        }
    }
}
