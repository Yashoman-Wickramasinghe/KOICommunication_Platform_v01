using KOICommunicationPlatform.Areas.Admin.Controllers;
using KOICommunicationPlatform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KOICommunicationPlatform.Areas.UniversityStudent.Controllers
{
    [Area("UniversityStudent")]
    public class StudentHomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public StudentHomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var user = _userManager.FindByIdAsync(userId)?.Result;

            // Check if the user is not null and has a valid name
            if (user != null)
            {
                // Pass the user's name to the view
                ViewBag.GivenName = user.GivenName;

                // Fetch the GroupGenerateId associated with the user
                var studentGroupDetail = _unitOfWork.StudentGroupDetail
                    .GetFirstOrDefault(sg => sg.Student.StudentId == user.StudentId, includeProperties: "Student");

                if (studentGroupDetail != null)
                {
                    // Count the number of members in this group
                    var groupMembers = _unitOfWork.StudentGroupDetail
                        .GetAll(sg => sg.GroupGenerateId == studentGroupDetail.GroupGenerateId, includeProperties: "Student")
                        .Select(sg => sg.Student).ToList();

                    ViewBag.GroupMemberCount = groupMembers.Count;
                    ViewBag.GroupMembers = groupMembers;
                }
                else
                {
                    ViewBag.GroupMemberCount = 0; // No group found for this user
                    ViewBag.GroupMembers = new List<Student>(); // Empty list for group members
                }
            }
            return View();
        }
    }
}
