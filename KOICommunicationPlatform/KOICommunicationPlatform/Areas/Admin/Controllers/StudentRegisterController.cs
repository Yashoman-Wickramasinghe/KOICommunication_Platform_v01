using KOICommunicationPlatform.DataAccess;
using KOICommunicationPlatform.Models;
using KOICommunicationPlatform.Models.ViewModels;
using KOICommunicationPlatform.Utilities;
using KOICommunicationPlatform.Utilities.EmailSender;
using KOICommunicationPlatform.Utilities.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using OfficeOpenXml;
using System.Text;

namespace KOICommunicationPlatform.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StudentRegisterController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IRegistrationEmailSender _registrationEmailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public StudentRegisterController(IUnitOfWork unitOfWork,
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
            // Fetch the list of ApplicationUsers including the related Tutorial, Subject, and Course data
            var _objStudentList = _unitOfWork.ApplicationUser.GetAll(
                includeProperties: "Tutorial,Tutorial.Subject,Tutorial.Subject.Course"
            ).Where(x => x.UserType == SD.Website_Student).ToList();

            return View(_objStudentList);
        }

        public IActionResult Create()
        {
            // Fetch tutorials with related Subject and Course data
            var tutorials = _unitOfWork.Tutorial.GetAll(includeProperties: "Subject,Subject.Course").Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = $"{(t.Subject?.Course?.CourseName ?? "N/A")} / {(t.Trimester ?? "N/A")} / {(t.Subject?.SubjectName ?? "N/A")} / {(t.Lab ?? "N/A")} / {(t.TutorialNo ?? "N/A")} / {(t.Day ?? "N/A")} / {(t.FromTime ?? "N/A")} / {(t.ToTime ?? "N/A")}"
            }).ToList();

            // Pass the Tutorials Dropdown list to the view via ViewBag
            ViewBag.TutorialsDropdown = tutorials;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(StudentRegistrationViewModel obj)
        {
            try
            {
                // Retrieve the selected Tutorial with related Subject and Course data
                var tutorial = _unitOfWork.Tutorial.GetFirstOrDefault(
                    t => t.Id == obj.TutorialId,
                    includeProperties: "Subject,Subject.Course"
                );

                var user = new ApplicationUser();

                user.StudentId = obj.Student.StudentId;
                user.GivenName = obj.Student.GivenName;
                user.Surname = obj.Student.Surname;
                user.Email = obj.Student.Email;
                user.UserName = obj.Student.Email;
                user.UserType = SD.Website_Student;
                user.CreatedDateTime = DateTime.Now;
                user.ModifieDateTime = DateTime.Now;
                user.Tutorial = tutorial;
                user.IsActive = true;

                _unitOfWork.ApplicationUser.Add(user);
                _unitOfWork.ApplicationUser.Save();
               
                var student = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Email == user.Email);

                //send registration email
                _registrationEmailSender.SendUserRegistrationEmail(student, $"{user.GivenName} {user.Surname}");

               

                // Create a new Student record
                var studentRecord = new Student
                {
                    StudentId = student.StudentId.ToString(),
                    GivenName = user.GivenName,
                    Surname = user.Surname,
                    IsActive = true,
                    CreatedDateTime = DateTime.Now,
                    ModifieDateTime = DateTime.Now,
                    Subject = tutorial.Subject,
                    Tutorial = tutorial
                };

                // Save the Student record
                _unitOfWork.Student.Add(studentRecord);
                _unitOfWork.Student.Save();

                TempData["success"] = "Student saved successfully";


                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> BulkCreate(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                var tutorial = new Tutorial();
                var subjectObj = new Subject();
                var users = new List<ApplicationUser>();
                var students = new List<Student>();

                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);

                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++) // Assuming the first row is the header
                        {
                            // Extract cell values and handle null or empty values
                            var studentId = worksheet.Cells[row, 1].Text;
                            var givenName = worksheet.Cells[row, 2].Text;
                            var surName = worksheet.Cells[row, 3].Text;
                            var email = worksheet.Cells[row, 4].Text;
                            var userName = email;
                            
                            var course = worksheet.Cells[row, 5].Text; 
                            var subject = worksheet.Cells[row, 6].Text;
                            var trimester = worksheet.Cells[row, 7].Text;
                            var lab  = worksheet.Cells[row, 8].Text; 
                            var tutorialNo = worksheet.Cells[row, 9].Text; 
                            var day = worksheet.Cells[row, 10].Text; 
                            var fromTime = worksheet.Cells[row, 11].Text; 
                            var toTime = worksheet.Cells[row, 12].Text;


                            // Check if required fields are not empty
                            if (string.IsNullOrWhiteSpace(studentId) || string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(email))
                            {
                                // Skip rows where required fields are missing
                                continue;
                            }

                            // Retrieve or create Course
                            var courseObj = _unitOfWork.Course.GetFirstOrDefault(c => c.CourseName == course);

                            if(courseObj != null)
                            {
                                // Retrieve or create Subject
                                subjectObj = _unitOfWork.Subject.GetFirstOrDefault(s => s.SubjectName == subject && s.CourseId == courseObj.Id);

                                if (subjectObj != null)
                                {
                                    // Retrieve or create Tutorial
                                    tutorial = _unitOfWork.Tutorial.GetFirstOrDefault(t => t.Subject.Id == subjectObj.Id && t.Day == day && t.FromTime == fromTime && t.ToTime == toTime && t.Trimester == trimester && t.Lab == lab && t.TutorialNo == tutorialNo);
                                }
                            }

                            // Create a new user only if required fields are present
                            var user = new ApplicationUser
                            {
                                StudentId = studentId,
                                GivenName = givenName,
                                Surname = surName,
                                UserName = userName,
                                Email = email,
                                UserType = SD.Website_Student,
                                CreatedDateTime = DateTime.Now,
                                ModifieDateTime = DateTime.Now,
                                Tutorial = tutorial,
                                IsActive = true,
                            };

                            users.Add(user);

                            if (tutorial != null)
                            {
                                // Create or update Student record
                                var student = new Student
                                {
                                    StudentId = studentId,
                                    GivenName = givenName,
                                    Surname = surName,
                                    IsActive = true,
                                    CreatedDateTime = DateTime.Now,
                                    ModifieDateTime = DateTime.Now,
                                    Subject = subjectObj,
                                    Tutorial = tutorial
                                };
                                
                                students.Add(student);
                            }
                        }
                    }
                }

                // Save all users to the database
                foreach (var user in users)
                {
                    _unitOfWork.ApplicationUser.Add(user);
                }

                 // Save all students to the database
                foreach (var student in students)
                {
                    _unitOfWork.Student.Add(student);
                }

                _unitOfWork.ApplicationUser.Save();
                _unitOfWork.Student.Save();

                TempData["success"] = "Students added successfully";

                
                foreach (var user in users)
                {
                    var student = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Email == user.Email);

                    //send registration email
                    _registrationEmailSender.SendUserRegistrationEmail(student, $"{user.GivenName} {user.Surname}");

                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return BadRequest(ex.Message);
            }
        }

        public IActionResult Edit(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Fetch the student from the database
            ApplicationUser? studentFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == id, includeProperties: "Tutorial");

            if (studentFromDb == null)
            {
                return NotFound();
            }

            // Fetch tutorials with related Subject and Course data
            var tutorials = _unitOfWork.Tutorial.GetAll(includeProperties: "Subject,Subject.Course")
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = $"{(t.Subject?.Course?.CourseName ?? "N/A")} / {(t.Trimester ?? "N/A")} / {(t.Subject?.SubjectName ?? "N/A")} / {(t.Lab ?? "N/A")} / {(t.TutorialNo ?? "N/A")} / {(t.Day ?? "N/A")} / {(t.FromTime?? "N/A")} / {(t.ToTime?? "N/A")}"
                }).ToList();

            // Create the view model
            var model = new StudentRegistrationViewModel
            {
                Student = studentFromDb,
                TutorialId = studentFromDb.Tutorial?.Id
            };

            // Pass the tutorials to the view via ViewBag
            ViewBag.TutorialsDropdown = tutorials;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, StudentRegistrationViewModel updatedStudent)
        {
            try
            {
                // Fetch the existing student from the ApplicationUser table
                ApplicationUser studentFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == id, includeProperties: "Tutorial");

                if (studentFromDb == null)
                {
                    return NotFound();
                }

                // Retrieve the selected Tutorial with related Subject and Course data
                var tutorial = _unitOfWork.Tutorial.GetFirstOrDefault(
                    t => t.Id == updatedStudent.TutorialId,
                    includeProperties: "Subject,Subject.Course"
                );

                // Update ApplicationUser properties
                studentFromDb.GivenName = updatedStudent.Student.GivenName;
                studentFromDb.Surname = updatedStudent.Student.Surname;
                studentFromDb.Email = updatedStudent.Student.Email;
                studentFromDb.Tutorial = tutorial; // Update the Tutorial
                studentFromDb.ModifieDateTime = DateTime.Now;
                studentFromDb.IsActive = true;

                _unitOfWork.ApplicationUser.Update(studentFromDb);

                // Fetch the existing student record from the Student table
                var studentRecord = _unitOfWork.Student.GetFirstOrDefault(s => s.StudentId == studentFromDb.StudentId);

                if (studentRecord != null)
                {
                    // Update Student properties
                    studentRecord.GivenName = studentFromDb.GivenName;
                    studentRecord.Surname = studentFromDb.Surname;
                    studentRecord.Tutorial = tutorial;
                    studentRecord.Subject = tutorial.Subject;
                    studentRecord.ModifieDateTime = DateTime.Now;

                    _unitOfWork.Student.Update(studentRecord);
                }

                _unitOfWork.ApplicationUser.Save(); // Save changes to ApplicationUser table
                _unitOfWork.Student.Save(); // Save changes to Student table

                TempData["success"] = "Student edited successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                TempData["error"] = $"An error occurred: {ex.Message}";
                return View(updatedStudent); // Return to the same view with error message
            }

        }

        public IActionResult Delete(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Fetch the student from the database
            ApplicationUser? studentFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == id, includeProperties: "Tutorial");

            if (studentFromDb == null)
            {
                return NotFound();
            }

            // Fetch tutorials with related Subject and Course data
            var tutorials = _unitOfWork.Tutorial.GetAll(includeProperties: "Subject,Subject.Course")
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = $"{(t.Subject?.Course?.CourseName ?? "N/A")} / {(t.Trimester ?? "N/A")} / {(t.Subject?.SubjectName ?? "N/A")} / {(t.Lab ?? "N/A")} / {(t.TutorialNo ?? "N/A")} / {(t.Day ?? "N/A")} / {(t.FromTime ?? "N/A")} / {(t.ToTime ?? "N/A")}"
                }).ToList();

            // Create the view model
            var model = new StudentRegistrationViewModel
            {
                Student = studentFromDb,
                TutorialId = studentFromDb.Tutorial?.Id
            };

            // Pass the tutorials to the view via ViewBag
            ViewBag.TutorialsDropdown = tutorials;

            return View(model);

        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(string id)
        {
            ApplicationUser? obj = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
         
            Student? student = _unitOfWork.Student.GetFirstOrDefault(u => u.StudentId == obj.StudentId);

            if (student == null)
            {
                return NotFound();
            }
            _unitOfWork.Student.Remove(student);
            _unitOfWork.Student.Save();

            _unitOfWork.ApplicationUser.Remove(obj);
            _unitOfWork.ApplicationUser.Save();

            TempData["success"] = "Student record deleted successfully";
            return RedirectToAction("Index");
        }


    }
}
