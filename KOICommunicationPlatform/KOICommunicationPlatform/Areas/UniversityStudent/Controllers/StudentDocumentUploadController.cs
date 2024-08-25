using KOICommunicationPlatform.DataAccess;
using KOICommunicationPlatform.Models;
using KOICommunicationPlatform.Models.ViewModels;
using KOICommunicationPlatform.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Areas.UniversityStudent.Controllers
{
    [Area("UniversityStudent")]
    public class StudentDocumentUploadController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        public StudentDocumentUploadController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch deliverables and documents from the database using UnitOfWork
            var deliverables = _unitOfWork.ProjectDeliverable.GetAll(d => d.IsActive).ToList();

            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            var student = _unitOfWork.Student.GetFirstOrDefault(x => x.StudentId == user.StudentId);

            var studentId = student.Id;
           
            //var studentGroupDetail = _unitOfWork.StudentGroupDetail.GetFirstOrDefault(x => x.Student.Id == studentId);


            // Fetch the studentGroupDetail including the related StudentGroupHD
            var studentGroupDetail = _unitOfWork.StudentGroupDetail
                .GetFirstOrDefault(
                    x => x.Student.Id == studentId, // Filter condition
                    includeProperties: "StudentGroupHD" // Include the related StudentGroupHD
                );

            if (studentGroupDetail != null && studentGroupDetail.StudentGroupHD != null)
            {
                // Safely access the StudentGroupHD.Id
                var groupHDId = studentGroupDetail.StudentGroupHD.Id;

                var uploadedStudent = _unitOfWork.Student.GetFirstOrDefault(x => x.StudentId == studentGroupDetail.Student.StudentId);

                // Proceed with fetching documents or other operations
                var documents = _unitOfWork.DocumentUpload
                    .GetAll(d => d.IsActive && d.StudentGroupHDId == groupHDId)
                    .ToList();


                var viewModel = new ProjectTimelineViewModel
                {
                    Deliverables = deliverables,
                    Documents = documents
                };

                return View(viewModel);
            }
            else
            {
                // Handle the case where studentGroupDetail or StudentGroupHD is null
                ModelState.AddModelError(string.Empty, "Unable to retrieve the group details.");
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument(IFormFile uploadedFile, int DeliverableId)
        {
            try
            {
                if (uploadedFile == null || uploadedFile.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                var userId = _userManager.GetUserId(User);
                var user = await _userManager.FindByIdAsync(userId);

                // Fetch deliverable and related entities
                var deliverable = _unitOfWork.ProjectDeliverable.GetFirstOrDefault(d => d.IsActive && d.Id == DeliverableId);
                if (deliverable == null)
                {
                    return NotFound("Deliverable not found.");
                }

                var course = _unitOfWork.Course.GetFirstOrDefault(x => x.Id == deliverable.CourseId);
                var student = _unitOfWork.Student.GetFirstOrDefault(x => x.StudentId == user.StudentId);
                //var studentGroup = _unitOfWork.StudentGroupDetail.GetFirstOrDefault(x => x.Student.Id == student.Id);
                //var userId1 = _userManager.GetUserId(User);
                //var user1 = await _userManager.FindByIdAsync(userId);

                //var student1 = _unitOfWork.Student.GetFirstOrDefault(x => x.StudentId == user.StudentId);

                var studentId = student.Id;

                var studentGroup = _unitOfWork.StudentGroupDetail
               .GetFirstOrDefault(
                   x => x.Student.Id == studentId, // Filter condition
                   includeProperties: "StudentGroupHD" // Include the related StudentGroupHD
               );

                if (course == null || student == null || studentGroup == null)
                {
                    return NotFound("Course, student, or student group not found.");
                }

                var coursePrefix = GetFirstLetter(course.CourseName);
                var groupId = studentGroup.GroupGenerateId;
                var groupFolderName = $"{coursePrefix}_{groupId}";
                var deliverableFolderName = $"{deliverable.DeliverableName}_{deliverable.SubjectId}";

                // Define base folder path and subfolders
                string baseFolderPath = Path.Combine(_hostEnvironment.WebRootPath, "Documents", "documentupload", groupFolderName, deliverableFolderName);

                // Create directories if they don't exist
                if (!Directory.Exists(baseFolderPath))
                {
                    Directory.CreateDirectory(baseFolderPath);
                }

                // Generate a unique file name with date and time
                string fileName = $"{Path.GetFileNameWithoutExtension(uploadedFile.FileName)}_{DateTime.Now:yyyyMMdd_HHmmss}{Path.GetExtension(uploadedFile.FileName)}";
                string filePath = Path.Combine(baseFolderPath, fileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(stream);
                }

                var documentUpload = new DocumentUpload();
                documentUpload.Version = filePath; // save file path is version
                documentUpload.Course = course;
                documentUpload.SubjectId = deliverable.SubjectId;
                documentUpload.CreatedDateTime = DateTime.Now;
                documentUpload.IsActive = true;
                documentUpload.ProjectDeliverableId = deliverable.Id;
                documentUpload.StudentGroupDetailId = studentGroup.Id;
                documentUpload.StudentGroupHDId = studentGroup.StudentGroupHD.Id; ;
                documentUpload.FullName = $"{student.GivenName} {student.Surname}";

                _unitOfWork.DocumentUpload.Add(documentUpload);
                _unitOfWork.DocumentUpload.Save();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception
                return BadRequest(ex.Message);
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

            var comments = _unitOfWork.CommentsOnDocumentUpload.GetAll(x=>x.DocumentId == documentId);

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
                CreatedBy = User.Identity.Name, // Or however you determine the commenter's name
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
