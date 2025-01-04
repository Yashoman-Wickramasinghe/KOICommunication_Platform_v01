using KOICommunicationPlatform.Models;
using KOICommunicationPlatform.Models.ViewModels;
using KOICommunicationPlatform.Utilities.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KOICommunicationPlatform.Controllers
{
    [Area("Admin")]
    public class TaskTrackingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IRegistrationEmailSender _registrationEmailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public TaskTrackingController(IUnitOfWork unitOfWork,
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
            // Prepare the tutorials dropdown
            var tutorials = _unitOfWork.Tutorial.GetAll(includeProperties: "Subject,Subject.Course").Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = $"{(t.Subject?.Course?.CourseName ?? "N/A")} / {(t.Trimester ?? "N/A")} / {(t.Subject?.SubjectName ?? "N/A")} / {(t.Lab ?? "N/A")} / {(t.TutorialNo ?? "N/A")} / {(t.Day ?? "N/A")} / {(t.FromTime ?? "N/A")} / {(t.ToTime ?? "N/A")}"
            }).ToList();

            ViewBag.TutorialsDropdown = tutorials;

            // Initialize the model with an empty list of ExistingSprints
            var sprintViewModel = new SprintViewModel
            {
                ExistingSprints = new List<Sprint>() // Ensure this is initialized to avoid null reference
            };

            return View(sprintViewModel);
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
        public async Task<IActionResult> ViewSprints(int? GroupId, int? TutorialId)
        {
            // Ensure GroupId is passed correctly
            var studentHDId = GroupId;

            // Fetch existing sprints for the selected group
            var existingSprints = _unitOfWork.Sprint.GetAll(
                s => s.StudentGroupHD.Id == studentHDId,
                includeProperties: "StudentGroupHD,Course").ToList();

            // Prepare the ViewModel
            var sprintViewModel = new SprintViewModel
            {
                ExistingSprints = existingSprints
            };

            // Return as partial view to load dynamically
            return PartialView("_SprintTasksAdminPartial", sprintViewModel);
        }


        [HttpPost]
        public IActionResult GetSprintTasks(int sprintId)
        {
            // Step 1: Get the UserId of the logged-in student
            var userId = _userManager.GetUserId(User);

            // Step 2: Retrieve the StudentId using the ApplicationUser model
            var applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == userId);
            if (applicationUser == null)
            {
                return NotFound("User not found.");
            }

            var sprintListObj = _unitOfWork.Sprint.GetFirstOrDefault(sgd => sgd.Id == sprintId);

            var studentHDId = sprintListObj.StudentGroupHDId;

            var studentGroupHD = _unitOfWork.StudentGroupHD.GetFirstOrDefault(sghd => sghd.Id == studentHDId, "Subject,Tutorial");
            if (studentGroupHD == null)
            {
                return NotFound("Student Group HD not found.");
            }

            var subjectId = studentGroupHD.Subject?.Id;
            var tutorialId = studentGroupHD.Tutorial?.Id;
            var groupGenerateId = studentGroupHD.GroupGenerateId;
            var studentGroupHdId = studentGroupHD.Id;

            // Fetching existing sprints
            var existingSprints = _unitOfWork.Sprint.GetAll(
               s => s.StudentGroupHD.Id == studentGroupHdId,
               includeProperties: "StudentGroupHD,Course").ToList();

            // Fetch group students
            var groupStudentDetails = _unitOfWork.StudentGroupDetail.GetAll(
                sgd => sgd.StudentGroupHD.Id == studentGroupHdId && sgd.IsActive,
                includeProperties: "Student"
            ).ToList();

            var groupStudents = groupStudentDetails
                                .Select(sgd => sgd.Student)
                                .Where(s => s != null)
                                .ToList();

            var sprintTasks = _unitOfWork.SprintTask.GetAll(
                st => st.SprintId == sprintId,
                includeProperties: "SprintTaskAssignments.Student"
            ).ToList();

            // Group tasks by their status
            var groupedTasks = sprintTasks
                .GroupBy(st => st.Status)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Define Status Options
            var statusOptions = new List<SelectListItem>
    {
        new SelectListItem { Value = "Backlog", Text = "Backlog" },
        new SelectListItem { Value = "ToDo", Text = "To Do" },
        new SelectListItem { Value = "InProgress", Text = "In Progress" },
        new SelectListItem { Value = "Done", Text = "Done" }
    };

            // Define Priority Options
            var priorityOptions = new List<SelectListItem>
    {
        new SelectListItem { Value = "High", Text = "High" },
        new SelectListItem { Value = "Medium", Text = "Medium" },
        new SelectListItem { Value = "Low", Text = "Low" }
    };

           // Ensure GroupedTasks is initialized even if no tasks are found
           var sprintViewModel = new SprintViewModel
           {
               StudentGroupHDId = studentGroupHdId,
               ExistingSprints = existingSprints,
               GroupStudents = groupStudents,
               StatusOptions = statusOptions,
               PriorityOptions = priorityOptions,
               GroupedTasks = groupedTasks ?? new Dictionary<string, List<SprintTask>>(), //Initialize to avoid null
               SelectedSprintId = sprintId,
               AssignedStudents = new Dictionary<int, List<StudentGroupViewModel>>()
           };

            // Populate AssignedStudents for each task
            foreach (var task in sprintTasks)
            {
                var assignedStudents = task.SprintTaskAssignments
                    .Select(a => new StudentGroupViewModel
                    {
                        StudentId = a.Student.StudentId,
                        GivenName = a.Student.GivenName,
                        Surname = a.Student.Surname,
                        Email = a.Student.Email
                    })
                    .ToList();

                // Add the list of assigned students to the ViewModel dictionary, keyed by SprintTaskId
                sprintViewModel.AssignedStudents[task.Id] = assignedStudents;
            }
            // Pass the ViewModel to the View
            return PartialView("_SprintTaskAdminSelectedSprint", sprintViewModel);
        }

        [HttpGet]
        public IActionResult GetComments(int taskId)
        {
            var comments = _unitOfWork.CommentsOnTaskBoard
                .GetAll(c => c.SprintTaskId == taskId && c.IsActive)
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

        [HttpPost]
        public IActionResult AddComment(int taskId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("Comment cannot be empty.");
            }

            // Logging for debugging
            Console.WriteLine($"Task ID: {taskId}, Content: {content}");

            var comment = new CommentsOnTaskBoard
            {
                SprintTaskId = taskId,
                Comment = content,
                CreatedBy = User.Identity.Name,
                CreatedDateTime = DateTime.Now,
                IsActive = true
            };

            _unitOfWork.CommentsOnTaskBoard.Add(comment);
            _unitOfWork.CommentsOnTaskBoard.Save();

            return Ok();
        }
    }
}
