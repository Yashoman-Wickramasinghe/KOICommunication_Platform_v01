using KOICommunicationPlatform.Models;
using KOICommunicationPlatform.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KOICommunicationPlatform.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubjectController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubjectController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            // Fetch all subjects and include the related course
            var subjectCourses = _unitOfWork.Subject.GetAll(includeProperties: "Course");
            return View(subjectCourses);
        }

        // GET: Subject/Create
        public IActionResult Create()
        {
            var viewModel = new SubjectViewModel
            {
                CourseList = _unitOfWork.Course.GetAll().Select(c => new SelectListItem
                {
                    Text = c.CourseName,
                    Value = c.Id.ToString()
                })
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SubjectViewModel obj)
        {
            if (obj.Subject.SubjectName != "")
            {
                var subject = new Subject
                {
                    SubjectName = obj.Subject.SubjectName,
                    CourseId = obj.Subject.CourseId,
                    IsActive = true,
                    CreatedDateTime = DateTime.Now,
                    ModifiedBy = "Admin",
                    ModifieDateTime = DateTime.Now
                };

                _unitOfWork.Subject.Add(subject);
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }

            obj.CourseList = _unitOfWork.Course.GetAll().Select(c => new SelectListItem
            {
                Text = c.CourseName,
                Value = c.Id.ToString()
            });
            return View(obj);
        }

        // GET: Subject/Edit/{id}
        public IActionResult Edit(int id)
        {
            var subject = _unitOfWork.Subject.GetFirstOrDefault(s => s.Id == id, includeProperties: "Course");
            if (subject == null)
            {
                return NotFound();
            }

            var obj = new SubjectViewModel
            {
                Subject = subject,
                CourseList = _unitOfWork.Course.GetAll().Select(c => new SelectListItem
                {
                    Text = c.CourseName,
                    Value = c.Id.ToString(),
                    Selected = c.Id == subject.CourseId
                })
            };

            return View(obj);
        }

        // POST: Subject/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SubjectViewModel obj)
        {
            if (obj.Subject.SubjectName != "")
            {
                _unitOfWork.Subject.Update(obj.Subject);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            obj.CourseList = _unitOfWork.Course.GetAll().Select(c => new SelectListItem
            {
                Text = c.CourseName,
                Value = c.Id.ToString(),
                Selected = c.Id == obj.Subject.CourseId
            });
            return View(obj);
        }

        // GET: Subject/Delete/{id}
        public IActionResult Delete(int id)
        {
            var subject = _unitOfWork.Subject.GetFirstOrDefault(s => s.Id == id, includeProperties: "Course");
            if (subject == null)
            {
                return NotFound();
            }

            var viewModel = new SubjectViewModel
            {
                Subject = subject,
                CourseList = _unitOfWork.Course.GetAll().Select(c => new SelectListItem
                {
                    Text = c.CourseName,
                    Value = c.Id.ToString(),
                    Selected = c.Id == subject.CourseId
                })
            };

            return View(viewModel);
        }

        // POST: Subject/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var subject = _unitOfWork.Subject.GetFirstOrDefault(s => s.Id == id);
            if (subject == null)
            {
                return NotFound();
            }

            _unitOfWork.Subject.Remove(subject);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
