using KOICommunicationPlatform.Areas.Admin.Controllers;
using KOICommunicationPlatform.Models;
using KOICommunicationPlatform.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

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
                LabTypeList = GetLabTypes().Select(l => new SelectListItem
                {
                    Text = l.Name,
                    Value = l.Code
                }),
                TutorialTypeList = GetTutorialTypes().Select(t => new SelectListItem
                {
                    Text = t.Name,
                    Value = t.Code
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
                var existingTutorial = _unitOfWork.Tutorial.GetAll()
                    .FirstOrDefault(t =>
                        t.Subject != null &&
                        t.Subject.Id == model.SubjectId &&
                        t.Day == model.Day &&
                        t.FromTime == model.FromTime &&
                        t.ToTime == model.ToTime &&
                        t.Trimester == model.Trimester &&
                        t.Lab == model.Lab && // Check LabType
                        t.TutorialNo == model.TutorialNo); // Check TutorialType

                if (existingTutorial != null)
                {
                    ModelState.AddModelError(string.Empty, "This tutorial already exists.");
                }
                else
                {
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
                            Lab = model.Lab, // Set LabType
                            TutorialNo = model.TutorialNo, // Set TutorialType
                            CreatedBy = User.Identity.Name,
                            IsActive = true
                        };

                        _unitOfWork.Tutorial.Add(tutorial);
                        _unitOfWork.Save();
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            // Re-populate the dropdowns in case of an error
            model.CourseList = _unitOfWork.Course.GetAll().Select(c => new SelectListItem
            {
                Text = c.CourseName,
                Value = c.Id.ToString()
            });
            model.LabTypeList = GetLabTypes().Select(l => new SelectListItem
            {
                Text = l.Name,
                Value = l.Code
            });
            model.TutorialTypeList = GetTutorialTypes().Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = t.Code
            });
            model.TrimesterList = GetTrimesters(DateTime.Now.Year);

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var tutorial = _unitOfWork.Tutorial.GetFirstOrDefault(t => t.Id == id, includeProperties: "Subject");

            if (tutorial == null)
            {
                return NotFound();
            }

            // Load hardcoded LabTypes and TutorialTypes
            var labTypes = GetLabTypes();
            var tutorialTypes = GetTutorialTypes();

            // Add the existing LabType and TutorialType from the database if not already in the list
            if (!labTypes.Any(l => l.Code == tutorial.Lab))
            {
                labTypes.Add(new LabType { Code = tutorial.Lab, Name = tutorial.Lab });
            }

            if (!tutorialTypes.Any(t => t.Code == tutorial.TutorialNo))
            {
                tutorialTypes.Add(new TutorialType { Code = tutorial.TutorialNo, Name = tutorial.TutorialNo });
            }

            var model = new TutorialViewModel
            {
                Id = tutorial.Id,
                Day = tutorial.Day,
                FromTime = tutorial.FromTime,
                ToTime = tutorial.ToTime,
                Trimester = tutorial.Trimester,
                SubjectId = tutorial.Subject.Id,
                CourseId = tutorial.Subject.CourseId,
                Lab = tutorial.Lab,
                TutorialNo = tutorial.TutorialNo,
                CourseList = _unitOfWork.Course.GetAll().Select(c => new SelectListItem
                {
                    Text = c.CourseName,
                    Value = c.Id.ToString()
                }),
                SubjectList = _unitOfWork.Subject.GetAll(s => s.CourseId == tutorial.Subject.CourseId).Select(s => new SelectListItem
                {
                    Text = s.SubjectName,
                    Value = s.Id.ToString(),
                    Selected = s.Id == tutorial.Subject.Id, // Ensure the selected subject is marked as selected
                }),
                LabTypeList = labTypes.Select(l => new SelectListItem
                {
                    Text = l.Name,
                    Value = l.Code,
                    Selected = l.Code == tutorial.Lab // Ensure the selected lab type is marked as selected
                }),
                TutorialTypeList = tutorialTypes.Select(t => new SelectListItem
                {
                    Text = t.Name,
                    Value = t.Code,
                    Selected = t.Code == tutorial.TutorialNo // Ensure the selected tutorial type is marked as selected
                }),
                TrimesterList = GetTrimesters(DateTime.Now.Year)
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TutorialViewModel model)
        {
            try
            {
                var existingTutorial = _unitOfWork.Tutorial.GetFirstOrDefault(
                    t => t.Id != model.Id &&
                         t.Subject.Id == model.SubjectId &&
                         t.Day == model.Day &&
                         t.FromTime == model.FromTime &&
                         t.ToTime == model.ToTime &&
                         t.Trimester == model.Trimester &&
                         t.Lab == model.Lab &&
                         t.TutorialNo == model.TutorialNo
                );

                if (existingTutorial != null)
                {
                    ModelState.AddModelError(string.Empty, "This tutorial already exists.");
                }
                else
                {
                    var tutorial = _unitOfWork.Tutorial.GetFirstOrDefault(t => t.Id == model.Id);

                    if (tutorial == null)
                    {
                        return NotFound();
                    }

                    var subject = _unitOfWork.Subject.GetFirstOrDefault(s => s.Id == model.SubjectId);

                    if (subject == null)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Subject selected.");
                    }
                    else
                    {
                        tutorial.Day = model.Day;
                        tutorial.FromTime = model.FromTime;
                        tutorial.ToTime = model.ToTime;
                        tutorial.Trimester = model.Trimester;
                        tutorial.Subject = subject;
                        tutorial.Lab = model.Lab;
                        tutorial.TutorialNo = model.TutorialNo;

                        _unitOfWork.Tutorial.Update(tutorial);
                        _unitOfWork.Save();

                        return RedirectToAction(nameof(Index));
                    }
                }

                // Re-populate the dropdowns in case of an error
                model.CourseList = _unitOfWork.Course.GetAll().Select(c => new SelectListItem
                {
                    Text = c.CourseName,
                    Value = c.Id.ToString()
                });
                model.SubjectList = _unitOfWork.Subject.GetAll(s => s.CourseId == model.CourseId).Select(s => new SelectListItem
                {
                    Text = s.SubjectName,
                    Value = s.Id.ToString()
                });
                model.LabTypeList = _unitOfWork.Tutorial.GetAll().Select(t => new SelectListItem
                {
                    Text = t.Lab,
                    Value = t.Lab
                }).Distinct();
                model.TutorialTypeList = _unitOfWork.Tutorial.GetAll().Select(t => new SelectListItem
                {
                    Text = t.TutorialNo,
                    Value = t.TutorialNo
                }).Distinct();
                model.TrimesterList = GetTrimesters(DateTime.Now.Year);

                return View(model);
            }
            catch (Exception)
            {
                // Handle the exception
                throw;
            }
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

        // GET: Tutorial/Delete/5
        public IActionResult Delete(int id)
        {
            var tutorial = _unitOfWork.Tutorial.GetFirstOrDefault(t => t.Id == id, includeProperties: "Subject,Subject.Course");

            if (tutorial == null || tutorial.Subject == null || tutorial.Subject.Course == null)
            {
                return NotFound();
            }

            var model = new TutorialViewModel
            {
                Id = tutorial.Id,
                Day = tutorial.Day,
                FromTime = tutorial.FromTime,
                ToTime = tutorial.ToTime,
                Trimester = tutorial.Trimester,
                SubjectId = tutorial.Subject.Id,
                CourseId = tutorial.Subject.CourseId,
                Lab = tutorial.Lab,
                TutorialNo = tutorial.TutorialNo,
                CourseName = tutorial.Subject.Course.CourseName,
                SubjectName = tutorial.Subject.SubjectName,
                CourseList = _unitOfWork.Course.GetAll().Select(c => new SelectListItem
                {
                    Text = c.CourseName,
                    Value = c.Id.ToString()
                }),
                SubjectList = _unitOfWork.Subject.GetAll(s => s.CourseId == tutorial.Subject.CourseId).Select(s => new SelectListItem
                {
                    Text = s.SubjectName,
                    Value = s.Id.ToString(),
                    Selected = s.Id == tutorial.Subject.Id,
                }),
                LabTypeList = GetLabTypes().Select(l => new SelectListItem
                {
                    Text = l.Name,
                    Value = l.Code,
                    Selected = l.Code == tutorial.Lab
                }),
                TutorialTypeList = GetTutorialTypes().Select(t => new SelectListItem
                {
                    Text = t.Name,
                    Value = t.Code,
                    Selected = t.Code == tutorial.TutorialNo
                }),
                TrimesterList = GetTrimesters(DateTime.Now.Year)
            };

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var tutorial = _unitOfWork.Tutorial.GetFirstOrDefault(t => t.Id == id);

            if (tutorial == null)
            {
                return NotFound();
            }

            _unitOfWork.Tutorial.Remove(tutorial);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }
    }
    public class LabType
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class TutorialType
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

}
