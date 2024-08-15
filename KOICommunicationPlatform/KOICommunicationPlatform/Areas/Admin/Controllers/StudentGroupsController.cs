using KOICommunicationPlatform.Models;
using KOICommunicationPlatform.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            var model = new TutorialViewModel
            {
                CourseList = _unitOfWork.Course.GetAll().Select(c => new SelectListItem
                {
                    Text = c.CourseName,
                    Value = c.Id.ToString()
                }).ToList(),
                SubjectList = new List<SelectListItem>(), // Initially empty
                TrimesterList = new List<SelectListItem>(), // Initially empty
                DayList = new List<SelectListItem>(), // Initially empty
                FromTimeList = new List<SelectListItem>(), // Initially empty
                ToTimeList = new List<SelectListItem>() // Initially empty
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
    }
}
