using KOICommunicationPlatform.DataAccess;
using KOICommunicationPlatform.Models;
using KOICommunicationPlatform.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace KOICommunicationPlatform.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProjectDeliverableController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;

        public ProjectDeliverableController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var currentYear = DateTime.Now.Year;
            var trimesters = GetTrimesters(currentYear);

            ProjectDeliverableViewModel projectDeliverableVM = new()
            {
                ProjectDeliverable = new ProjectDeliverable
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1)
                },
                CourseList = _unitOfWork.Course.GetAll().Select(i => new SelectListItem
                {
                    Text = i.CourseName,
                    Value = i.Id.ToString()
                }),
                SubjectList = _unitOfWork.Subject.GetAll().Select(i => new SelectListItem
                {
                    Text = i.SubjectName,
                    Value = i.Id.ToString()
                }),
                TrimesterList = trimesters
            };

            return View(projectDeliverableVM);
        }

        //public IActionResult Upsert(int? id)
        //{
        //    var currentYear = DateTime.Now.Year;
        //    var trimesters = GetTrimesters(currentYear);

        //    ProjectDeliverableViewModel projectDeliverableVM = new()
        //    {
        //        ProjectDeliverable = new ProjectDeliverable
        //        {
        //            StartDate = DateTime.Now,
        //            EndDate = DateTime.Now.AddDays(1)
        //        },
        //        CourseList = _unitOfWork.Course.GetAll().Select(i => new SelectListItem
        //        {
        //            Text = i.CourseName,
        //            Value = i.Id.ToString()
        //        }),
        //        SubjectList = _unitOfWork.Subject.GetAll().Select(i => new SelectListItem
        //        {
        //            Text = i.SubjectName,
        //            Value = i.Id.ToString()
        //        }),
        //        TrimesterList = trimesters
        //    };

        //    if (id == null || id == 0)
        //    {
        //        return View(projectDeliverableVM);
        //    }
        //    else
        //    {
        //        projectDeliverableVM.ProjectDeliverable = _unitOfWork.ProjectDeliverable.GetFirstOrDefault(u => u.Id == id);
        //        return View(projectDeliverableVM);
        //    }
        //}
        // Upsert Action
        public IActionResult Upsert(int? id)
        {
            var currentYear = DateTime.Now.Year;
            var trimesters = GetTrimesters(currentYear);

            var projectDeliverable = id == null || id == 0
                ? new ProjectDeliverable { StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1) }
                : _unitOfWork.ProjectDeliverable.GetFirstOrDefault(u => u.Id == id);

            var projectDeliverableVM = new ProjectDeliverableViewModel
            {
                ProjectDeliverable = projectDeliverable,
                CourseList = _unitOfWork.Course.GetAll().Select(i => new SelectListItem
                {
                    Text = i.CourseName,
                    Value = i.Id.ToString()
                }),

                SubjectList = id == null || id == 0
                    ? new List<SelectListItem>()
                    : _unitOfWork.Subject.GetAll().Where(s => s.CourseId == projectDeliverable.CourseId)
                      .Select(i => new SelectListItem
                      {
                          Text = i.SubjectName,
                          Value = i.Id.ToString()
                      }),

                TrimesterList = trimesters
            };

            return View(projectDeliverableVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProjectDeliverableViewModel obj)
        {
            var currentYear = DateTime.Now.Year;
            var trimesters = GetTrimesters(currentYear);

            if (obj.ProjectDeliverable.Id == 0)
                {
                    obj.ProjectDeliverable.CourseId = obj.CourseId;
                    obj.ProjectDeliverable.SubjectId = obj.SubjectId;
                    _unitOfWork.ProjectDeliverable.Add(obj.ProjectDeliverable);
                }
                else
                {
                    obj.ProjectDeliverable.CourseId = obj.CourseId;
                    obj.ProjectDeliverable.SubjectId = obj.SubjectId;
                    _unitOfWork.ProjectDeliverable.Update(obj.ProjectDeliverable);
                }
                _unitOfWork.Save();
                TempData["success"] = "Project Deliverable created successfully";
                return RedirectToAction("Index");

            return View(obj);
        }

        [HttpGet]
        public IActionResult GetSubjectsByCourseId(int courseId)
        {
            var subjects = _unitOfWork.Subject.GetAll()
                .Where(s => s.CourseId == courseId)
                .Select(s => new SelectListItem
                {
                    Text = s.SubjectName,
                    Value = s.Id.ToString()
                });

            return Json(subjects);
        }

        private IEnumerable<SelectListItem> GetTrimesters(int year)
        {
            var trimesters = new List<SelectListItem>
            {
                new SelectListItem { Text = $"T1{year.ToString().Substring(2)}", Value = $"T1{year.ToString().Substring(2)}" },
                new SelectListItem { Text = $"T2{year.ToString().Substring(2)}", Value = $"T2{year.ToString().Substring(2)}" },
                new SelectListItem { Text = $"T3{year.ToString().Substring(2)}", Value = $"T3{year.ToString().Substring(2)}" }
            };

            return trimesters;
        }

        [HttpGet]
        public IActionResult GetFilteredData(int courseId, string trimester, int subjectId)
        {
            var filteredData = _unitOfWork.ProjectDeliverable.GetAll(includeProperties: "Course,Subject")
                .Where(pd => (courseId == 0 || pd.CourseId == courseId) &&
                             (string.IsNullOrEmpty(trimester) || pd.Trimester == trimester) &&
                             (subjectId == 0 || pd.SubjectId == subjectId))
                .ToList();

            var result = filteredData.Select(pd => new
            {
                pd.Id,
                pd.DeliverableName,
                pd.StartDate,
                pd.EndDate,
                pd.Trimester,
                CourseName = pd.Course.CourseName,
                SubjectName = pd.Subject.SubjectName
            }).ToList();

            return Json(new { data = result });
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var _objProjectDeliverableList = _unitOfWork.ProjectDeliverable.GetAll(includeProperties: "Course,Subject").ToList();

            var result = _objProjectDeliverableList.Select(pd => new
            {
                pd.Id,
                pd.DeliverableName,
                pd.StartDate,
                pd.EndDate,
                pd.Trimester,
                CourseName = pd.Course.CourseName,
                SubjectName = pd.Subject.SubjectName
            }).ToList();

            return Json(new { data = result });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.ProjectDeliverable.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.ProjectDeliverable.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}

