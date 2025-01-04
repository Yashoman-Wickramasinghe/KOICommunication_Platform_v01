using KOICommunicationPlatform.Areas.Admin.Controllers;
using KOICommunicationPlatform.Models;
using KOICommunicationPlatform.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KOICommunicationPlatform.Areas.Client.Controllers
{
    [Area("Client")]
    public class ClientDocumentUploadController : Controller
    {
        private readonly ILogger<ClientDocumentUploadController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ClientDocumentUploadController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Fetch deliverables and documents from the database using UnitOfWork
                var deliverables = _unitOfWork.ProjectDeliverable.GetAll(d => d.IsActive).ToList();

                var userIdString = _userManager.GetUserId(User);
                if (!Guid.TryParse(userIdString, out var userId))
                {
                    // Handle the case where userId cannot be converted to Guid
                    return BadRequest("Invalid User ID");
                }

                // Retrieve the StudentGroupHD record where ClientId matches the Guid
                var studentHdIdObj = _unitOfWork.StudentGroupHD.GetFirstOrDefault(x => x.ClientId == userId);

                var studentHdId = studentHdIdObj.Id;

                var GetStudentDetails = _unitOfWork.StudentGroupDetail.GetAll(x => x.StudentGroupHD.Id == studentHdId).ToList();

                // Proceed with fetching documents or other operations
                var GetUploadedDocumentsList = _unitOfWork.DocumentUpload.GetAll(x => x.StudentGroupHDId == studentHdId).ToList();

                var viewModel = new ProjectTimelineViewModel
                {
                    Deliverables = deliverables,
                    Documents = GetUploadedDocumentsList
                };
                return View(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
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

