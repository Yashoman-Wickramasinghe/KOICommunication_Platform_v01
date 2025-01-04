using KOICommunicationPlatform.Models;
using KOICommunicationPlatform.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KOICommunicationPlatform.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StudentGroupsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentGroupsController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var year = DateTime.Now.Year;

            // Fetch tutorials with related Subject and Course data
            var tutorials = _unitOfWork.Tutorial.GetAll(includeProperties: "Subject,Subject.Course").Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = $"{(t.Subject?.Course?.CourseName ?? "N/A")} / {(t.Trimester ?? "N/A")} / {(t.Subject?.SubjectName ?? "N/A")} / {(t.Lab ?? "N/A")} / {(t.TutorialNo ?? "N/A")} / {(t.Day ?? "N/A")} / {(t.FromTime ?? "N/A")} / {(t.ToTime ?? "N/A")}"
            }).ToList();

            // Fetch clients
            var clients = _unitOfWork.ApplicationUser.GetAll(u => u.UserType == "Client");
                
            var clientList = clients.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Organization // or any other property from ApplicationUser
                }).ToList();
   
            
            // Prepare the ViewModel
            var viewModel = new TutorialViewModel
            {
                CourseList = _unitOfWork.Course.GetAll().Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CourseName
                }).ToList(),

                SubjectList = _unitOfWork.Subject.GetAll().Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.SubjectName
                }).ToList(),

                TrimesterList = GetTrimesters(year),

                LabTypeList = new List<SelectListItem>
        {
            new SelectListItem { Value = "LA", Text = "LA" },
            new SelectListItem { Value = "LB", Text = "LB" },
            new SelectListItem { Value = "LC", Text = "LC" }
        },

                TutorialTypeList = new List<SelectListItem>
        {
            new SelectListItem { Value = "T1", Text = "T1" },
            new SelectListItem { Value = "T2", Text = "T2" },
            new SelectListItem { Value = "T3", Text = "T3" },
            new SelectListItem { Value = "T4", Text = "T4" }
        },

                DayList = new List<SelectListItem>
        {
            new SelectListItem { Value = "Monday", Text = "Monday" },
            new SelectListItem { Value = "Tuesday", Text = "Tuesday" },
            new SelectListItem { Value = "Wednesday", Text = "Wednesday" },
            new SelectListItem { Value = "Thursday", Text = "Thursday" },
            new SelectListItem { Value = "Friday", Text = "Friday" },
            new SelectListItem { Value = "Saturday", Text = "Saturday" }
        },

                FromTimeList = _unitOfWork.Tutorial.GetAll()
                    .Select(t => t.FromTime)
                    .Distinct()
                    .OrderBy(time => time)
                    .Select(time => new SelectListItem
                    {
                        Value = time,
                        Text = time // Format if needed
                    }).ToList(),

                ToTimeList = _unitOfWork.Tutorial.GetAll()
                    .Select(t => t.ToTime)
                    .Distinct()
                    .OrderBy(time => time)
                    .Select(time => new SelectListItem
                    {
                        Value = time,
                        Text = time // Format if needed
                    }).ToList(),

                ClientList = clientList
            };

            // Pass the Tutorials Dropdown list to the view via ViewBag
            ViewBag.TutorialsDropdown = tutorials;
            return View(viewModel);
        }


        [HttpPost]
        public IActionResult GetSubjects(int courseId)
        {
            var subjects = _unitOfWork.Subject.GetAll(s => s.CourseId == courseId)
                                               .Select(s => new SelectListItem
                                               {
                                                   Text = s.SubjectName,
                                                   Value = s.Id.ToString()
                                               }).ToList();
            return Json(subjects);
        }

        [HttpPost]
        public IActionResult GetFilteredData(int courseId, int subjectId)
        {
            var tutorials = _unitOfWork.Tutorial.GetAll(t => t.Subject.CourseId == courseId && t.Subject.Id == subjectId);

            var trimesters = tutorials.Select(t => t.Trimester)
                                      .Distinct()
                                      .Select(tr => new SelectListItem
                                      {
                                          Text = tr,
                                          Value = tr
                                      }).ToList();

            var days = tutorials.Select(t => t.Day)
                                .Distinct()
                                .Select(d => new SelectListItem
                                {
                                    Text = d,
                                    Value = d
                                }).ToList();

            var fromTimes = tutorials.Select(t => t.FromTime)
                                     .Distinct()
                                     .Select(ft => new SelectListItem
                                     {
                                         Text = ft,
                                         Value = ft
                                     }).ToList();

            var toTimes = tutorials.Select(t => t.ToTime)
                                   .Distinct()
                                   .Select(tt => new SelectListItem
                                   {
                                       Text = tt,
                                       Value = tt
                                   }).ToList();
            return Json(new
            {
                trimesters,
                days,
                fromTimes,
                toTimes
            });
        }

        private List<LabType> GetLabTypes()
        {
            return new List<LabType>{
                new LabType { Code = "LA", Name = "Lab A" },
                new LabType { Code = "LB", Name = "Lab B" },
                new LabType { Code = "LC", Name = "Lab C" },
                new LabType { Code = "LD", Name = "Lab D" }
            };
        }

        private List<TutorialType> GetTutorialTypes()
        {
            return new List<TutorialType>{
                new TutorialType { Code = "T1", Name = "Tutorial 1" },
                new TutorialType { Code = "T2", Name = "Tutorial 2" },
                new TutorialType { Code = "T3", Name = "Tutorial 3" },
                new TutorialType { Code = "T4", Name = "Tutorial 4" }
            };
        }

        private IEnumerable<SelectListItem> GetTrimesters(int year)
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = $"T1{year.ToString().Substring(2)}", Value = $"T1{year.ToString().Substring(2)}" },
                new SelectListItem { Text = $"T2{year.ToString().Substring(2)}", Value = $"T2{year.ToString().Substring(2)}" },
                new SelectListItem { Text = $"T3{year.ToString().Substring(2)}", Value = $"T3{year.ToString().Substring(2)}" }
            };
        }

        [HttpGet]
        public JsonResult GetTutorialDetails(int tutorialId)
        {
            try
            {
                var tutorial = _unitOfWork.Tutorial.GetFirstOrDefault(t => t.Id == tutorialId, includeProperties: "Subject,Subject.Course");

                if (tutorial == null)
                {
                    return Json(null);
                }

                return Json(new
                {
                    courseName = tutorial.Subject?.Course?.CourseName,
                    courseId = tutorial.Subject?.Course?.Id,  // Add course ID
                    trimester = tutorial.Trimester,
                    subjectName = tutorial.Subject?.SubjectName,
                    subjectId = tutorial.Subject?.Id,  // Add subject ID
                    lab = tutorial.Lab,
                    tutorialNo = tutorial.TutorialNo,
                    day = tutorial.Day,
                    fromTime = tutorial.FromTime,
                    toTime = tutorial.ToTime
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public JsonResult GenerateGroupIdAndGetStudents(int tutorialId)
        {
            try
            {
                // Step 1: Retrieve the tutorial, including related properties
                var tutorial = _unitOfWork.Tutorial.GetFirstOrDefault(
                    t => t.Id == tutorialId,
                    includeProperties: "Subject,Subject.Course");

                if (tutorial == null)
                {
                    return Json(new
                    {
                        GroupId = "Error: Tutorial not found.",
                        Students = new List<SelectListItem> { new SelectListItem { Text = "Error: Tutorial not found.", Value = string.Empty } }
                    });
                }

                // Step 2: Generate Group ID
                var courseId = tutorial.Subject?.Course?.Id;
                var subjectId = tutorial.Subject?.Id;
                var trimester = tutorial.Trimester;
                var day = tutorial.Day;
                var labType = tutorial.Lab;
                var tutorialType = tutorial.TutorialNo;

                // Step 3: Fetch existing student groups that match the criteria
                var existingGroups = _unitOfWork.StudentGroupHD.GetAll(sg =>
                    sg.Subject.Id == subjectId &&
                    sg.Trimester == trimester &&
                    sg.Tutorial.Day == day &&
                    sg.GroupGenerateId.StartsWith(labType + tutorialType));

                // Calculate the new group number
                int count = existingGroups.Any() ? existingGroups.Count() + 1 : 1;
                string groupId = $"{labType}{tutorialType}-{count}";

                // Step 4: Check if the Student repository is initialized
                if (_unitOfWork.Student == null)
                {
                    throw new InvalidOperationException("Student repository is not initialized.");
                }

                // Step 5: Retrieve students related to the selected tutorial
                var studentsQuery = _unitOfWork.Student
                    .GetAll(s => s.Tutorial != null && s.Tutorial.Id == tutorialId)
                    .ToList(); // Fetch the students first into a list (memory operation)

                // Step 6: Filter out the students who are already in StudentGroupDetail
                var filteredStudents = studentsQuery
                    .Where(s => !_unitOfWork.StudentGroupDetail.GetAll(g => g.StudentId == s.Id).Any())
                    .ToList(); // Filter the results in memory

                if (!filteredStudents.Any())
                {
                    return Json(new
                    {
                        GroupId = groupId, // Return the GroupId even if no students are found
                        Students = new List<SelectListItem> { new SelectListItem { Text = "No students found.", Value = string.Empty } }
                    });
                }

                // Step 7: Map students to SelectListItem
                var studentList = filteredStudents
                    .Select(s => new SelectListItem
                    {
                        Text = $"{s.GivenName} {s.Surname} ({s.StudentId})",
                        Value = s.Id.ToString()
                    }).ToList();

                // Step 7: Return Group ID and Student List
                return Json(new
                {
                    GroupId = groupId,
                    Students = studentList
                });
            }
            catch (Exception ex)
            {
                // Log the exception details here if necessary
                return Json(new
                {
                    GroupId = $"Error: {ex.Message}",
                    Students = new List<SelectListItem> { new SelectListItem { Text = $"Error: {ex.Message}", Value = string.Empty } }
                });
            }
        }

        [HttpPost]
        public JsonResult SaveStudentGroup(int tutorialId, List<int> studentIds, int? leaderId, string clientId)
        {
            try
            {
                // Step 1: Retrieve the tutorial based on tutorialId
                var tutorial = _unitOfWork.Tutorial.GetFirstOrDefault(t => t.Id == tutorialId, includeProperties: "Subject,Subject.Course");

                if (tutorial == null)
                {
                    return Json(new { Success = false, Message = "Error: Tutorial not found." });
                }

                // Step 2: Generate Group ID
                var subjectId = tutorial.Subject?.Id;
                var trimester = tutorial.Trimester;
                var day = tutorial.Day;
                var labType = tutorial.Lab;
                var tutorialType = tutorial.TutorialNo;

                // Fetch existing groups to determine new group count
                var existingGroups = _unitOfWork.StudentGroupHD.GetAll(sg =>
                    sg.Subject.Id == subjectId &&
                    sg.Trimester == trimester &&
                    sg.Tutorial.Day == day &&
                    sg.GroupGenerateId.StartsWith(labType + tutorialType));

                // New group number
                int count = existingGroups.Any() ? existingGroups.Count() + 1 : 1;
                string groupId = $"{labType}{tutorialType}-{count}";

                var userId = _userManager.GetUserId(User);
                
                // Step 3: Create and save StudentGroupHD
                var studentGroupHD = new StudentGroupHD
                {
                    GroupGenerateId = groupId,
                    SubjectId = subjectId.Value,
                    Trimester = trimester,
                    TutorialId = tutorialId,
                    CreatedDateTime = DateTime.Now,
                    ClientId = Guid.Parse(clientId),
                    IsActive = true,
                    CreatedBy = userId
                    //LeaderId = leaderId // Optional leader ID
                };

                _unitOfWork.StudentGroupHD.Add(studentGroupHD);
                _unitOfWork.Save();  // Save to generate the ID

                // Step 4: Add each student to StudentGroupDetails
                if (studentIds != null && studentIds.Count > 0)
                {
                    foreach (var studentId in studentIds)
                    {
                        var studentGroupDetail = new StudentGroupDetail
                        {
                            StudentGroupHDId = studentGroupHD.Id,  // Link to StudentGroupHD
                            StudentId = studentId,
                            GroupGenerateId = groupId,
                            IsActive=true,
                            CreatedBy = userId,
                            IsLeader = leaderId.HasValue && studentId == leaderId.Value  
                        };
                        _unitOfWork.StudentGroupDetail.Add(studentGroupDetail);
                    }
                    _unitOfWork.Save();  // Save StudentGroupDetails
                }
                return Json(new { Success = true, GroupId = groupId });
            }
            catch (Exception ex)
            {
                // Return error in case of exception
                return Json(new { Success = false, Message = $"Error: {ex.Message}" });
            }
        }
    }
}
