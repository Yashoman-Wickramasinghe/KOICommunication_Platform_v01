using KOICommunicationPlatform.Areas.Admin.Controllers;
using KOICommunicationPlatform.Models;
using KOICommunicationPlatform.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KOICommunicationPlatform.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TutorialController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public TutorialController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Tutorial/Index
        public async Task<IActionResult> Index()
        {
            var tutorials = _unitOfWork.Tutorial.GetAll(includeProperties: "Subject,Subject.Course").ToList();
            return View(tutorials);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new TutorialViewModel
            {
                CourseList = _unitOfWork.Course.GetAll().Select(c => new SelectListItem
                {
                    Text = c.CourseName,
                    Value = c.Id.ToString()
                }),
                TrimesterList = GetTrimesters(DateTime.Now.Year)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TutorialViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the tutorial already exists
                var existingTutorial = _unitOfWork.Tutorial.GetAll()
                    .FirstOrDefault(t =>
                        t.Subject != null &&
                        t.Subject.Id == model.SubjectId &&
                        t.Day == model.Day &&
                        t.FromTime == model.FromTime &&
                        t.ToTime == model.ToTime &&
                        t.Trimester == model.Trimester);

                if (existingTutorial != null)
                {
                    ModelState.AddModelError(string.Empty, "This tutorial already exists.");
                }
                else
                {
                    // Fetch the Subject and check if it exists
                    var subject = _unitOfWork.Subject.GetFirstOrDefault(s => s.Id == model.SubjectId);

                    if (subject == null)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Subject selected.");
                    }
                    else
                    {
                        var tutorial = new Tutorial
                        {
                            Day = model.Day,
                            FromTime = model.FromTime,
                            ToTime = model.ToTime,
                            Trimester = model.Trimester,
                            Subject = subject,
                            CreatedBy = User.Identity.Name,
                            IsActive = true
                        };

                        _unitOfWork.Tutorial.Add(tutorial);
                        _unitOfWork.Save();
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            // Re-populate dropdowns if validation fails
            model.CourseList = _unitOfWork.Course.GetAll().Select(c => new SelectListItem
            {
                Text = c.CourseName,
                Value = c.Id.ToString()
            });
            model.TrimesterList = GetTrimesters(DateTime.Now.Year);

            return View(model);
        }


        public JsonResult GetSubjectsByCourse(int courseId)
        {
            var subjects = _unitOfWork.Subject.GetAll(s => s.CourseId == courseId).Select(s => new SelectListItem
            {
                Text = s.SubjectName,
                Value = s.Id.ToString()
            });
            return Json(subjects);
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
        public IActionResult Edit(int id)
        {
            var tutorial = _unitOfWork.Tutorial.GetFirstOrDefault(t => t.Id == id, includeProperties: "Subject,Subject.Course");
            if (tutorial == null)
            {
                return NotFound();
            }

            var viewModel = new TutorialViewModel
            {
                Id = tutorial.Id,
                Day = tutorial.Day,
                FromTime = tutorial.FromTime,
                ToTime = tutorial.ToTime,
                CourseId = tutorial.Subject.CourseId,
                SubjectId = tutorial.Subject.Id,
                Trimester = tutorial.Trimester,
                CourseList = _unitOfWork.Course.GetAll().Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CourseName
                }),
                SubjectList = _unitOfWork.Subject.GetAll(s => s.CourseId == tutorial.Subject.CourseId).Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.SubjectName
                }),
                TrimesterList = GetTrimesters(DateTime.Now.Year)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TutorialViewModel model)
        {
            if (model.FromTime != "" && model.ToTime != "" && model.Day != "")
            {
                // Check if the tutorial with the same details already exists (excluding the current tutorial)
                var existingTutorials = _unitOfWork.Tutorial.GetAll(includeProperties: "Subject")
                    .Where(t => t.Subject != null &&
                                t.Subject.Id == model.SubjectId &&
                                t.Day == model.Day &&
                                t.FromTime == model.FromTime &&
                                t.ToTime == model.ToTime &&
                                t.Trimester == model.Trimester &&
                                t.Id != model.Id);

                if (existingTutorials.Any())
                {
                    ModelState.AddModelError(string.Empty, "A tutorial with these details already exists.");
                    return View(model);
                }

                var tutorialFromDb = _unitOfWork.Tutorial.GetFirstOrDefault(t => t.Id == model.Id, includeProperties: "Subject");
                if (tutorialFromDb == null)
                {
                    return NotFound();
                }

                // Update the tutorial details
                tutorialFromDb.Day = model.Day;
                tutorialFromDb.FromTime = model.FromTime;
                tutorialFromDb.ToTime = model.ToTime;
                tutorialFromDb.ModifiedBy = User.Identity.Name;
                tutorialFromDb.ModifieDateTime = DateTime.Now;

                _unitOfWork.Tutorial.Update(tutorialFromDb);
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }

            // Reload dropdowns in case of an error
            model.CourseList = _unitOfWork.Course.GetAll().Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CourseName
            });

            model.SubjectList = _unitOfWork.Subject.GetAll(s => s.CourseId == model.CourseId).Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.SubjectName
            });

            model.TrimesterList = GetTrimesters(DateTime.Now.Year);

            return View(model);
        }

        // GET: Tutorial/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var tutorial = _unitOfWork.Tutorial.GetFirstOrDefault(t => t.Id == id, includeProperties: "Subject,Subject.Course");
            if (tutorial == null)
            {
                return NotFound();
            }

            var viewModel = new TutorialViewModel
            {
                Id = tutorial.Id,
                Day = tutorial.Day,
                FromTime = tutorial.FromTime,
                ToTime = tutorial.ToTime,
                Trimester = tutorial.Trimester,
                CourseName = tutorial.Subject?.Course?.CourseName,
                SubjectName = tutorial.Subject?.SubjectName
            };

            return View(viewModel);
        }

        // POST: Tutorial/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var tutorial = _unitOfWork.Tutorial.GetFirstOrDefault(t => t.Id == id, includeProperties: "Subject,Subject.Course");
            if (tutorial == null)
            {
                return NotFound();
            }

            _unitOfWork.Tutorial.Remove(tutorial);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

    }
}
