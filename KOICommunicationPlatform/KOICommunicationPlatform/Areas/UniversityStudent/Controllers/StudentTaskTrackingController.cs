using KOICommunicationPlatform.Models;
using KOICommunicationPlatform.Models.ViewModels;
using KOICommunicationPlatform.Utilities.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace KOICommunicationPlatform.Areas.UniversityStudent.Controllers
{
    [Area("UniversityStudent")]
    public class StudentTaskTrackingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IRegistrationEmailSender _registrationEmailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<StudentTaskTrackingController> _logger;

        public StudentTaskTrackingController(IUnitOfWork unitOfWork,
         IWebHostEnvironment hostEnvironment,
         IRegistrationEmailSender registrationEmailSender,
         UserManager<ApplicationUser> userManager,
         IConfiguration configuration, ILogger<StudentTaskTrackingController> logger)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
            _registrationEmailSender = registrationEmailSender;
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }
        public IActionResult Index()
        {
            // Step 1: Get the UserId of the logged-in student
            var userId = _userManager.GetUserId(User);

            // Step 2: Retrieve the StudentId using the ApplicationUser model
            var applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == userId);
            if (applicationUser == null)
            {
                return NotFound("User not found.");
            }
            var studentId = applicationUser.StudentId;

            // Step 3: Get the Id from the Student model using the StudentId
            var student = _unitOfWork.Student.GetFirstOrDefault(s => s.StudentId == studentId);
            if (student == null)
            {
                return NotFound("Student not found.");
            }
            var studentInternalId = student.Id;

            // Step 4: Retrieve the StudentHDId from the StudentGroupDetail model using the StudentId
            var studentGroupDetail = _unitOfWork.StudentGroupDetail.GetFirstOrDefault(sgd => sgd.Student.Id == studentInternalId, "StudentGroupHD");
            if (studentGroupDetail == null)
            {
                return NotFound("Student Group Detail not found.");
            }
            var studentHDId = studentGroupDetail.StudentGroupHD.Id;

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

            // Prepare the ViewModel
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

            // Pass the ViewModel to the View
            return View(sprintViewModel);
        }

        [HttpPost]
        public IActionResult AddSprint(int studentGroupHDId, string sprintName, DateTime startDate, DateTime endDate)
        {
            // Step 5: Use the StudentHDId to fetch the SubjectId, TutorialId, and GroupGenerateId from the StudentGroupHD model
            var studentGroupHD = _unitOfWork.StudentGroupHD.GetFirstOrDefault(sghd => sghd.Id == studentGroupHDId, "Subject,Tutorial");

            if (studentGroupHD == null)
            {
                return NotFound("Student Group HD not found.");
            }
            var subjectId = studentGroupHD.Subject?.Id;
            //var getCourseId = _unitOfWork.Subject.GetFirstOrDefault(x=>x.Id == subjectId);
            var getCourse = _unitOfWork.Subject.GetFirstOrDefault(x => x.Id == subjectId, includeProperties: "Course");

            if (getCourse == null || getCourse.Course == null)
            {
                return NotFound("Course not found for the selected subject.");
            }
            // Create TaskBoard entry
            var taskBoard = new TaskBoard
            {
                SubjectId = (int)subjectId,
                CourseId = getCourse.Course.Id, // Assign the CourseId
                IsActive = true,
                CreatedBy = _userManager.GetUserName(User),
                CreatedDateTime = DateTime.Now,
                ModifieDateTime = DateTime.Now,
                Course = getCourse.Course // Assign the Course object
            };

            _unitOfWork.TaskBoard.Add(taskBoard);
            _unitOfWork.Save();

            // Create Sprint entry
            var sprint = new Sprint
            {
                SprintName = sprintName,
                StartDate = startDate,
                EndDate = endDate,
                SubjectId = taskBoard.SubjectId,
                Trimester = _unitOfWork.StudentGroupHD.GetFirstOrDefault(x => x.Id == studentGroupHDId).Trimester,
                IsActive = true,
                CreatedBy = _userManager.GetUserName(User),
                CreatedDateTime = DateTime.Now,
                ModifieDateTime = DateTime.Now,
                Course = taskBoard.Course,
                StudentGroupHD = _unitOfWork.StudentGroupHD.GetFirstOrDefault(x => x.Id == studentGroupHDId)
            };
            _unitOfWork.Sprint.Add(sprint);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTask(SprintViewModel model, int selectedSprintId)
        {
            if (model.SelectedStudentIds == null || !model.SelectedStudentIds.Any())
            {
                ModelState.AddModelError("", "Please select at least one student to assign the task.");
            }

            if (string.IsNullOrWhiteSpace(model.TaskDescription))
            {
                ModelState.AddModelError("TaskDescription", "Description is required.");
            }

            if (string.IsNullOrWhiteSpace(model.SelectedStatus))
            {
                ModelState.AddModelError("SelectedStatus", "Status is required.");
            }

            if (string.IsNullOrWhiteSpace(model.SelectedPriority))
            {
                ModelState.AddModelError("SelectedPriority", "Priority is required.");
            }

            if (model.TaskStartDate == default || model.TaskEndDate == default)
            {
                ModelState.AddModelError("", "Start Date and End Date are required.");
            }

            if (!ModelState.IsValid)
            {
                // Re-populate the ViewModel
                // Step 1: Get the UserId of the logged-in student
                var userId = _userManager.GetUserId(User);

                // Step 2: Retrieve the StudentId using the ApplicationUser model
                var applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == userId);
                if (applicationUser == null)
                {
                    return NotFound("User not found.");
                }
                var studentId = applicationUser.StudentId;

                // Step 3: Get the Id from the Student model using the StudentId
                var student = _unitOfWork.Student.GetFirstOrDefault(s => s.StudentId == studentId);
                if (student == null)
                {
                    return NotFound("Student not found.");
                }
                var studentInternalId = student.Id;

                // Step 4: Retrieve the StudentHDId from the StudentGroupDetail model using the StudentId
                var studentGroupDetail = _unitOfWork.StudentGroupDetail.GetFirstOrDefault(sgd => sgd.Student.Id == studentInternalId, "StudentGroupHD");
                if (studentGroupDetail == null)
                {
                    return NotFound("Student Group Detail not found.");
                }
                var studentHDId = studentGroupDetail.StudentGroupHD.Id;

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
                   s => s.StudentGroupHD.Id == studentGroupHdId).ToList();

                // Fetch group students
                var groupStudentDetails = _unitOfWork.StudentGroupDetail.GetAll(
                    sgd => sgd.StudentGroupHD.Id == studentGroupHdId && sgd.IsActive,
                    includeProperties: "Student"
                ).ToList();

                var groupStudents = groupStudentDetails
                                    .Select(sgd => sgd.Student)
                                    .Where(s => s != null)
                                    .ToList();

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

                // Re-populate the ViewModel
                var sprintViewModel = new SprintViewModel
                {
                    //Id = 
                    StudentGroupHDId = studentGroupHdId,
                    ExistingSprints = existingSprints,
                    GroupStudents = groupStudents,
                    StatusOptions = statusOptions,
                    PriorityOptions = priorityOptions,
                    TaskName = model.TaskName,
                    TaskDescription = model.TaskDescription,
                    SelectedStatus = model.SelectedStatus,
                    SelectedPriority = model.SelectedPriority,
                    TaskStartDate = model.TaskStartDate,
                    TaskEndDate = model.TaskEndDate,
                    SelectedStudentIds = model.SelectedStudentIds
                };

                //return View("Index", sprintViewModel);
            }

            // Find the active sprint for the group
            var sprint = _unitOfWork.Sprint.GetFirstOrDefault(s => s.StudentGroupHD.Id == model.StudentGroupHDId && s.IsActive && s.Id == selectedSprintId, includeProperties: "SprintTasks");
            if (sprint == null)
            {
                return NotFound("Active sprint not found.");
            }

            // Create a new SprintTask
            var sprintTask = new SprintTask
            {
                TaskName = model.TaskName,
                Description = model.TaskDescription,
                Status = model.SelectedStatus,
                Priority = model.SelectedPriority,
                StartDate = model.TaskStartDate,
                EndDate = model.TaskEndDate,
                SprintId = sprint.Id,
                IsActive = true,
                Sprint = sprint  
            };

            _unitOfWork.SprintTask.Add(sprintTask);
            _unitOfWork.Save();

            // Assign the task to selected students
            foreach (var studentId in model.SelectedStudentIds)
            {
                var assignment = new SprintTaskAssignment
                {
                    SprintTaskId = sprintTask.Id,
                    IsActive = true,
                    StudentId = studentId
                };
                _unitOfWork.SprintTaskAssignment.Add(assignment);
            }

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
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
            var studentId = applicationUser.StudentId;

            // Step 3: Get the Id from the Student model using the StudentId
            var student = _unitOfWork.Student.GetFirstOrDefault(s => s.StudentId == studentId);
            if (student == null)
            {
                return NotFound("Student not found.");
            }
            var studentInternalId = student.Id;

            // Step 4: Retrieve the StudentHDId from the StudentGroupDetail model using the StudentId
            var studentGroupDetail = _unitOfWork.StudentGroupDetail.GetFirstOrDefault(sgd => sgd.Student.Id == studentInternalId, "StudentGroupHD");
            if (studentGroupDetail == null)
            {
                return NotFound("Student Group Detail not found.");
            }
            var studentHDId = studentGroupDetail.StudentGroupHD.Id;

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
            return PartialView("_SprintTasksPartial", sprintViewModel);
        }

        [HttpPost]
        public IActionResult EditTask(string taskInfo)
        {
            if (string.IsNullOrEmpty(taskInfo))
            {
                return Json(new { success = false, message = "Invalid task information." });
            }

            var parts = taskInfo.Split('_');
            if (parts.Length != 2 || !int.TryParse(parts[0], out int taskId) || !int.TryParse(parts[1], out int sprintId))
            {
                return Json(new { success = false, message = "Failed to parse task information." });
            }

            var userId = _userManager.GetUserId(User);
            var applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == userId);
            if (applicationUser == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var student = _unitOfWork.Student.GetFirstOrDefault(s => s.StudentId == applicationUser.StudentId);
            if (student == null)
            {
                return Json(new { success = false, message = "Student not found." });
            }

            var studentGroupDetail = _unitOfWork.StudentGroupDetail.GetFirstOrDefault(sgd => sgd.Student.Id == student.Id, "StudentGroupHD");
            if (studentGroupDetail == null)
            {
                return Json(new { success = false, message = "Student Group Detail not found." });
            }

            var task = _unitOfWork.SprintTask.GetFirstOrDefault(st => st.Id == taskId && st.SprintId == sprintId, includeProperties: "SprintTaskAssignments.Student");
            if (task == null)
            {
                return Json(new { success = false, message = "Task not found." });
            }

            var studentGroupId = studentGroupDetail.StudentGroupHD.Id;

            // Fetching existing sprints
            var existingSprints = _unitOfWork.Sprint.GetAll(
               s => s.StudentGroupHD.Id == studentGroupId,
               includeProperties: "StudentGroupHD,Course").ToList();

            var sprintOptions = existingSprints.Select(s => new
            {
                s.Id,
                s.SprintName,
                IsSelected = s.Id == sprintId // Mark the correct sprint as selected
            }).ToList();

            var groupStudents = _unitOfWork.StudentGroupDetail.GetAll(sgd => sgd.StudentGroupHD.Id == studentGroupId && sgd.IsActive, includeProperties: "Student")
                .Select(sgd => new
                {
                    sgd.Student.Id,
                    sgd.Student.StudentId,
                    sgd.Student.GivenName,
                    sgd.Student.Surname,
                    sgd.Student.Email,
                    IsSelected = task.SprintTaskAssignments.Any(a => a.StudentId == sgd.Student.Id)
                }).ToList();

            var response = new
            {
                success = true,
                taskId = task.Id,
                sprintId = task.SprintId,
                sprintOptions = sprintOptions,
                students = groupStudents,
                selectedStatus = task.Status,
                statusOptions = new[]
                {
            new { value = "Backlog", text = "Backlog" },
            new { value = "ToDo", text = "To Do" },
            new { value = "InProgress", text = "In Progress" },
            new { value = "Done", text = "Done" }
        },
                taskName = task.TaskName,
                taskDescription = task.Description,
                selectedPriority = task.Priority,
                priorityOptions = new[]
                {
            new { value = "High", text = "High" },
            new { value = "Medium", text = "Medium" },
            new { value = "Low", text = "Low" }
        },
                taskStartDate = task.StartDate.ToString("yyyy-MM-ddTHH:mm"),
                taskEndDate = task.EndDate.ToString("yyyy-MM-ddTHH:mm")
            };

            return Json(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateTask(SprintViewModel model)
        {
            if (model.SelectedStudentIds == null || !model.SelectedStudentIds.Any())
            {
                ModelState.AddModelError("", "Please select at least one student to assign the task.");
            }

            if (string.IsNullOrWhiteSpace(model.TaskDescription))
            {
                ModelState.AddModelError("TaskDescription", "Description is required.");
            }

            if (string.IsNullOrWhiteSpace(model.SelectedStatus))
            {
                ModelState.AddModelError("SelectedStatus", "Status is required.");
            }

            if (string.IsNullOrWhiteSpace(model.SelectedPriority))
            {
                ModelState.AddModelError("SelectedPriority", "Priority is required.");
            }

            if (model.TaskStartDate == default || model.TaskEndDate == default)
            {
                ModelState.AddModelError("", "Start Date and End Date are required.");
            }

            //if (!ModelState.IsValid)
            //{
            //    // Re-populate the ViewModel and return if not valid
            //    // Same as CreateTask method for re-populating the ViewModel
            //    // (Code omitted for brevity)
            //    return View("Index", model);
            //}

            var sprintTask = _unitOfWork.SprintTask.GetFirstOrDefault(st => st.Id == model.TaskId);

            if (sprintTask == null)
            {
                return NotFound("Task not found.");
            }

            // Update task details
            sprintTask.TaskName = model.TaskName;
            sprintTask.Description = model.TaskDescription;
            sprintTask.Status = model.SelectedStatus;
            sprintTask.Priority = model.SelectedPriority;
            sprintTask.StartDate = model.TaskStartDate;
            sprintTask.EndDate = model.TaskEndDate;
            sprintTask.SprintId = model.SprintId;

            _unitOfWork.SprintTask.Update(sprintTask);

            // Get current student assignments for the task
            var existingAssignments = _unitOfWork.SprintTaskAssignment.GetAll(assignment => assignment.SprintTaskId == sprintTask.Id).ToList();

            // Determine which assignments to remove
            //var selectedStudentIds = model.SelectedStudentIds.ToHashSet();
            HashSet<int> selectedStudentIds = model.SelectedStudentIds != null
            ? model.SelectedStudentIds.ToHashSet(): new HashSet<int>();

            var assignmentsToRemove = existingAssignments.Where(a => !selectedStudentIds.Contains(a.StudentId)).ToList();

            foreach (var assignment in assignmentsToRemove)
            {
                _unitOfWork.SprintTaskAssignment.Remove(assignment);
            }

            // Determine which assignments to add
            var existingStudentIds = existingAssignments.Select(a => a.StudentId).ToHashSet();
            var studentIdsToAdd = selectedStudentIds.Except(existingStudentIds);

            foreach (var studentId in studentIdsToAdd)
            {
                var newAssignment = new SprintTaskAssignment
                {
                    SprintTaskId = sprintTask.Id,
                    IsActive = true,
                    StudentId = studentId
                };
                _unitOfWork.SprintTaskAssignment.Add(newAssignment);
            }

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
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

        [HttpPost]
        [Route("StudentTaskTracking/DeleteSprint/{sprintId}")]
        public IActionResult DeleteSprint(int sprintId)
        {
            try
            {
                // Step 1: Get all sprint tasks associated with the sprint
                var sprintTasks = _unitOfWork.SprintTask.GetAll(st => st.SprintId == sprintId).ToList();

                // Step 2: Delete related CommentsOnTaskBoard by SprintTaskId
                foreach (var task in sprintTasks)
                {
                    var commentsOnTaskBoard = _unitOfWork.CommentsOnTaskBoard.GetAll(c => c.SprintTaskId == task.Id).ToList();
                    if (commentsOnTaskBoard.Any())
                    {
                        _unitOfWork.CommentsOnTaskBoard.RemoveRange(commentsOnTaskBoard);
                    }
                }

                // Step 3: Delete SprintTaskAssignments by SprintTaskId
                foreach (var task in sprintTasks)
                {
                    var taskAssignments = _unitOfWork.SprintTaskAssignment.GetAll(ta => ta.SprintTaskId == task.Id).ToList();
                    if (taskAssignments.Any())
                    {
                        _unitOfWork.SprintTaskAssignment.RemoveRange(taskAssignments);
                    }
                }

                // Step 4: Delete SprintTasks by SprintId
                if (sprintTasks.Any())
                {
                    _unitOfWork.SprintTask.RemoveRange(sprintTasks);
                }

                // Step 5: Finally, delete the Sprint itself
                var sprint = _unitOfWork.Sprint.GetFirstOrDefault(s => s.Id == sprintId);
                if (sprint != null)
                {
                    _unitOfWork.Sprint.Remove(sprint);
                }

                // Save the changes to the database
                _unitOfWork.Save();

                return Ok(new { message = "Sprint and related data deleted successfully!" });
            }
            catch (Exception ex)
            {
                // Return error response
                return StatusCode(500, "Error deleting sprint: " + ex.Message);
            }
        }

        //
        [HttpPost]
        [Route("StudentTaskTracking/DeleteSprintTask/{taskId}")]
        public IActionResult DeleteSprintTask(int taskId)
        {
            try
            {
                var commentsOnTaskBoard = _unitOfWork.CommentsOnTaskBoard.GetAll(c => c.SprintTaskId == taskId).ToList();
                
                if (commentsOnTaskBoard.Any())
                
                {
                    _unitOfWork.CommentsOnTaskBoard.RemoveRange(commentsOnTaskBoard);
                }
  
                var taskAssignments = _unitOfWork.SprintTaskAssignment.GetAll(ta=>ta.SprintTaskId == taskId).ToList();

                 if (taskAssignments.Any())
                 {
                     _unitOfWork.SprintTaskAssignment.RemoveRange(taskAssignments);
                 }

                var sprintTasks = _unitOfWork.SprintTask.GetFirstOrDefault(st => st.Id == taskId);

                _unitOfWork.SprintTask.Remove(sprintTasks);

                _unitOfWork.Save();

                return Ok(new { message = "Sprint Task and related data deleted successfully!" });
            }
            catch (Exception ex)
            {
                // Return error response
                return StatusCode(500, "Error deleting sprint: " + ex.Message);
            }
        }

    }
}
