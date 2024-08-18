using KOICommunicationPlatform.Models;
using KOICommunicationPlatform.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KOICommunicationPlatform.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StudentGroupsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentGroupsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var year = DateTime.Now.Year; // Get the current year for trimesters
            var model = new TutorialViewModel
            {
                CourseList = _unitOfWork.Course.GetAll().Select(c => new SelectListItem
                {
                    Text = c.CourseName,
                    Value = c.Id.ToString()
                }).ToList(),
                SubjectList = new List<SelectListItem>(), // Initially empty
                TrimesterList = GetTrimesters(year).ToList(), // Populate trimesters based on the current year
                DayList = new List<SelectListItem>(), // Initially empty
                FromTimeList = new List<SelectListItem>(), // Initially empty
                ToTimeList = new List<SelectListItem>(), // Initially empty
                LabTypeList = GetLabTypes().Select(l => new SelectListItem
                {
                    Text = l.Name,
                    Value = l.Code
                }).ToList(),
                TutorialTypeList = GetTutorialTypes().Select(t => new SelectListItem
                {
                    Text = t.Name,
                    Value = t.Code
                }).ToList()
            };
            return View(model);
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

        [HttpPost]
        public JsonResult GenerateGroupId(int courseId, int subjectId, string trimester,string day, string fromTime, string toTime, string labType, string tutorialType)
        {
            // Fetch existing records based on the filters
            var existingGroups = _unitOfWork.StudentGroupHD.GetAll(sg =>
                sg.CourseName == courseId.ToString() &&
                sg.Subject == subjectId.ToString() &&
                sg.Trimester == trimester && // Adjust as needed
                sg.TutorialSession == day &&
                sg.GroupId.StartsWith(labType + tutorialType));

            int count = existingGroups.Any() ? existingGroups.Count() + 1 : 1;

            // Generate Group ID
            string groupId = $"{labType}{tutorialType}-{count}";

            //bool isExist = _unitOfWork.StudentGroupHD.IsExist(x=>x.GroupId == groupId);

            return Json(new { groupId });
        }
    }
}
