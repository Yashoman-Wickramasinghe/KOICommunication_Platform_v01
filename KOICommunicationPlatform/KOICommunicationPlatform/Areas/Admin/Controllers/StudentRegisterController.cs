using KOICommunicationPlatform.DataAccess;
using KOICommunicationPlatform.Models;
using KOICommunicationPlatform.Utilities;
using KOICommunicationPlatform.Utilities.EmailSender;
using KOICommunicationPlatform.Utilities.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            // Fetch the list of students
            var _objStudentList = _unitOfWork.ApplicationUser.GetAll().Where(x => x.UserType == SD.Website_Student).ToList();
            return View(_objStudentList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser obj)
        {
            try
            {
                obj.UserName = obj.Email;
                obj.UserType = SD.Website_Student;
                obj.CreatedDateTime = DateTime.Now;
                obj.ModifieDateTime = DateTime.Now;
                obj.IsActive = true;

                _unitOfWork.ApplicationUser.Add(obj);
                _unitOfWork.ApplicationUser.Save();
                TempData["success"] = "Student saved successfully";

                var student = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Email == obj.Email);

                //send registration email
                _registrationEmailSender.SendUserRegistrationEmail(student, $"{obj.GivenName} {obj.Surname}");

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
                var users = new List<ApplicationUser>();

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
                            var contactName = worksheet.Cells[row, 3].Text;
                            var phoneNumber = worksheet.Cells[row, 4].Text;
                            var contactPerson02Name = worksheet.Cells[row, 5].Text;
                            var contactPerson02Phone = worksheet.Cells[row, 6].Text;
                            var documentLink = worksheet.Cells[row, 7].Text;

                            // Check if required fields are not empty
                            if (string.IsNullOrWhiteSpace(studentId) || string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(email))
                            {
                                // Skip rows where required fields are missing
                                continue;
                            }

                            // Create a new user only if required fields are present
                            var user = new ApplicationUser
                            {
                                StudentId = studentId,
                                GivenName = givenName,
                                Surname = surName,
                                UserName = userName,
                                Email = email,
                                ContactName = contactName,
                                PhoneNumber = phoneNumber,
                                ContactPerson02Name = contactPerson02Name,
                                ContactPerson02Phone = contactPerson02Phone,
                                DocumentLink = documentLink,
                                UserType = SD.Website_Student,
                                CreatedDateTime = DateTime.Now,
                                ModifieDateTime = DateTime.Now,
                                IsActive = true,
                            };

                            users.Add(user);
                        }
                    }
                }

                // Save all users to the database
                foreach (var user in users)
                {
                    _unitOfWork.ApplicationUser.Add(user);
                }

                _unitOfWork.ApplicationUser.Save();

                TempData["success"] = "Students added successfully";

                //TODO: Uncomment
                foreach (var user in users)
                {
                    var student = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Email == user.Email);

                    //send registration email
                    _registrationEmailSender.SendUserRegistrationEmail(student, user.Organization);

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

            ApplicationUser? studentFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == id);

            if (studentFromDb == null)
            {
                return NotFound();
            }

            return View(studentFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, ApplicationUser updatedStudent)
        {
            try
            {
                // Fetch the existing student from the database
                ApplicationUser studentFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == id);

                if (studentFromDb == null)
                {
                    return NotFound();
                }

                // Update student properties from the form values
                studentFromDb.GivenName = updatedStudent.GivenName;
                studentFromDb.Surname = updatedStudent.Surname;
                studentFromDb.Email = updatedStudent.Email;
                studentFromDb.ContactName = updatedStudent.ContactName;
                studentFromDb.ContactPhone = updatedStudent.ContactPhone;
                studentFromDb.ContactPerson02Name = updatedStudent.ContactPerson02Phone = updatedStudent.ContactPerson02Phone;
                studentFromDb.ModifieDateTime = DateTime.Now;
                studentFromDb.IsActive = true;

                _unitOfWork.ApplicationUser.Update(studentFromDb);
                _unitOfWork.ApplicationUser.Save();
                TempData["success"] = "Student edited successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        public IActionResult Delete(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            ApplicationUser? studentFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == id);

            if (studentFromDb == null)
            {
                return NotFound();
            }

            return View(studentFromDb);

        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(string id)
        {
            ApplicationUser? obj = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.ApplicationUser.Remove(obj);
            _unitOfWork.ApplicationUser.Save();
            TempData["success"] = "Student record deleted successfully";
            return RedirectToAction("Index");
        }


    }
}
