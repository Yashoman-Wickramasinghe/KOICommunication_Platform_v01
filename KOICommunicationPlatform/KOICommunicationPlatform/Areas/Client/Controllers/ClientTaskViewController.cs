using KOICommunicationPlatform.Models;
using KOICommunicationPlatform.Models.ViewModels;
using KOICommunicationPlatform.Utilities.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KOICommunicationPlatform.Areas.Client.Controllers
{
    [Area("Client")]
    public class ClientTaskViewController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IRegistrationEmailSender _registrationEmailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public ClientTaskViewController(IUnitOfWork unitOfWork,
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
            // Step 1: Get the UserId of the logged-in client
            var userId = _userManager.GetUserId(User);

            // Step 2: Retrieve the ApplicationUser model using the UserId
            var applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == userId);
            if (applicationUser == null)
            {
                return NotFound("Client not found.");
            }

            // Step 3: Retrieve the StudentGroupHD using the client's ID (ClientId)
            var clientId = applicationUser.Id; // Assuming clientId is mapped in ApplicationUser

            // Retrieve the StudentGroupHD record associated with this client
            var studentGroupHD = _unitOfWork.StudentGroupHD.GetFirstOrDefault(
                sghd => sghd.ClientId.ToString() == clientId && sghd.IsActive,
                includeProperties: "Subject,Tutorial");

            if (studentGroupHD == null)
            {
                return NotFound("Student Group not found for the client.");
            }

            var studentGroupHdId = studentGroupHD.Id;

            // Step 4: Use the StudentGroupHDId to fetch the SubjectId, TutorialId, and GroupGenerateId
            var subjectId = studentGroupHD.Subject?.Id;
            var tutorialId = studentGroupHD.Tutorial?.Id;
            var groupGenerateId = studentGroupHD.GroupGenerateId;

            // Step 5: Fetch existing sprints associated with this StudentGroupHD
            var existingSprints = _unitOfWork.Sprint.GetAll(
                s => s.StudentGroupHD.Id == studentGroupHdId,
                includeProperties: "StudentGroupHD,Course").ToList();

            // Step 6: Fetch group students
            var groupStudentDetails = _unitOfWork.StudentGroupDetail.GetAll(
                sgd => sgd.StudentGroupHD.Id == studentGroupHdId && sgd.IsActive,
                includeProperties: "Student").ToList();

            var groupStudents = groupStudentDetails
                                .Select(sgd => sgd.Student)
                                .Where(s => s != null)
                                .ToList();

            // Step 7: Fetch tasks and assignments for the first sprint
            var firstSprintId = existingSprints.FirstOrDefault()?.Id ?? 0;

            var sprintTasks = _unitOfWork.SprintTask.GetAll(
                st => st.SprintId == firstSprintId,
                includeProperties: "SprintTaskAssignments.Student").ToList();

            // Step 8: Group tasks by their status
            var groupedTasks = sprintTasks
                .GroupBy(st => st.Status)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Step 9: Define Status and Priority Options
            var statusOptions = new List<SelectListItem>
    {
        new SelectListItem { Value = "Backlog", Text = "Backlog" },
        new SelectListItem { Value = "ToDo", Text = "To Do" },
        new SelectListItem { Value = "InProgress", Text = "In Progress" },
        new SelectListItem { Value = "Done", Text = "Done" }
    };

            var priorityOptions = new List<SelectListItem>
    {
        new SelectListItem { Value = "High", Text = "High" },
        new SelectListItem { Value = "Medium", Text = "Medium" },
        new SelectListItem { Value = "Low", Text = "Low" }
    };

            // Step 10: Prepare the ViewModel
            var sprintViewModel = new SprintViewModel
            {
                StudentGroupHDId = studentGroupHdId,
                ExistingSprints = existingSprints,
                GroupStudents = groupStudents,
                StatusOptions = statusOptions,
                PriorityOptions = priorityOptions,
                GroupedTasks = groupedTasks,
                SelectedSprintId = firstSprintId,
                AssignedStudents = new Dictionary<int, List<StudentGroupViewModel>>()
            };

            // Step 11: Populate AssignedStudents for each task
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

            // Step 12: Pass the ViewModel to the View
            return View(sprintViewModel);
        }

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
            //var studentId = applicationUser.StudentId;

            //// Step 3: Get the Id from the Student model using the StudentId
            //var student = _unitOfWork.Student.GetFirstOrDefault(s => s.StudentId == studentId);
            //if (student == null)
            //{
            //    return NotFound("Student not found.");
            //}
            //var studentInternalId = student.Id;

            // Step 4: Retrieve the StudentHDId from the StudentGroupDetail model using the StudentId
            //var studentGroupDetail = _unitOfWork.StudentGroupDetail.GetFirstOrDefault(sgd => sgd.Student.Id == studentInternalId, "StudentGroupHD");
            //if (studentGroupDetail == null)
            //{
            //    return NotFound("Student Group Detail not found.");
            //}
            var sprintListObj = _unitOfWork.Sprint.GetFirstOrDefault(sgd => sgd.Id == sprintId);

            var studentHDId = sprintListObj.StudentGroupHDId;
            //var studentHDId = studentGroupDetail.StudentGroupHD.Id;

            // Step 5: Use the StudentHDId to fetch the SubjectId, TutorialId, and GroupGenerateId from the StudentGroupHD model
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

            // Fetch tasks and assignments for the first sprint
            //var firstSprintId = existingSprints.FirstOrDefault()?.Id ?? 0;

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

            // Prepare the ViewModel
            var sprintViewModel = new SprintViewModel
            {
                StudentGroupHDId = studentGroupHdId,
                ExistingSprints = existingSprints,
                GroupStudents = groupStudents,
                StatusOptions = statusOptions,
                PriorityOptions = priorityOptions,
                GroupedTasks = groupedTasks,
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
            return PartialView("_SprintTasksClientViewPartial", sprintViewModel);
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
