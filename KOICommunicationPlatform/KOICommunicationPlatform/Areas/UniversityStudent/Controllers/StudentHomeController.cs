using KOICommunicationPlatform.Areas.Admin.Controllers;
using KOICommunicationPlatform.Models;
using KOICommunicationPlatform.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            // Get the logged-in user's ID
            var userId = _userManager.GetUserId(User);
            var user = _userManager.FindByIdAsync(userId)?.Result;

            if (user != null)
            {
                // Pass the user's GivenName to the view
                ViewBag.GivenName = user.GivenName;

                // Retrieve the student details using the user's StudentId
                var student = _unitOfWork.Student
                    .GetFirstOrDefault(s => s.StudentId == user.StudentId, includeProperties: "Subject,Tutorial");

                if (student != null)
                {
                    // Fetch the project deliverables associated with the student's course and subject
                    var projectDeliverables = _unitOfWork.ProjectDeliverable
                        .GetAll(pd => pd.CourseId == student.Tutorial.Subject.CourseId && pd.SubjectId == student.Subject.Id)
                        .ToList();

                    ViewBag.ProjectDeliverables = projectDeliverables;
                    ViewBag.ProjectDeliverableCount = projectDeliverables.Count;

                    // Fetch the GroupGenerateId associated with the user
                    var studentGroupDetail = _unitOfWork.StudentGroupDetail
                        .GetFirstOrDefault(sg => sg.Student.StudentId == user.StudentId, includeProperties: "Student");

                    if (studentGroupDetail != null)
                    {
                        // Get Group Members
                        var groupMembers = _unitOfWork.StudentGroupDetail
                            .GetAll(sg => sg.GroupGenerateId == studentGroupDetail.GroupGenerateId, includeProperties: "Student")
                            .Select(sg => sg.Student).ToList();

                        ViewBag.GroupMemberCount = groupMembers.Count;
                        ViewBag.GroupMembers = groupMembers;

                        // Get Client Details
                        var studentGroupHD = _unitOfWork.StudentGroupHD
                            .GetFirstOrDefault(sg => sg.GroupGenerateId == studentGroupDetail.GroupGenerateId);

                        if (studentGroupHD != null && studentGroupHD.ClientId.HasValue)
                        {
                            var client = _unitOfWork.ApplicationUser
                                .GetFirstOrDefault(c => c.Id == studentGroupHD.ClientId.ToString());

                            ViewBag.ClientDetails = client;
                        }

                        // --- Merging the logic from GetUserTasks ---

                        // Use the StudentHDId to fetch the SubjectId, TutorialId, and GroupGenerateId from the StudentGroupHD model
                        if (studentGroupHD != null)
                        {
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
                            var firstSprintId = existingSprints.FirstOrDefault()?.Id ?? 0;

                            var sprintTasks = _unitOfWork.SprintTask.GetAll(
                                st => st.SprintId == firstSprintId,
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

                            // Prepare the ViewModel for tasks
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

                            // Pass the sprint tasks ViewModel to the view
                            ViewBag.SprintViewModel = sprintViewModel;
                        }
                    }
                    else
                    {
                        ViewBag.GroupMemberCount = 0;
                        ViewBag.GroupMembers = new List<Student>();
                    }
                }
                else
                {
                    ViewBag.ProjectDeliverables = new List<ProjectDeliverable>();
                    ViewBag.ProjectDeliverableCount = 0;
                    ViewBag.GroupMemberCount = 0;
                    ViewBag.GroupMembers = new List<Student>();
                }
            }
            else
            {
                ViewBag.ProjectDeliverables = new List<ProjectDeliverable>();
                ViewBag.ProjectDeliverableCount = 0;
                ViewBag.GroupMemberCount = 0;
                ViewBag.GroupMembers = new List<Student>();
            }

            return View();
        }

    }
}
