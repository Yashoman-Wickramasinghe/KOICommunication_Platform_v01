using KOICommunicationPlatform.Models;
using Microsoft.AspNetCore.Mvc;

namespace KOICommunicationPlatform.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CourseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Course> objCourseList = _unitOfWork.Course.GetAll();
            return View(objCourseList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Course obj)
        {
            try
            {
                obj.CreatedBy = "Admin";
                obj.CreatedDateTime = DateTime.Now;
                obj.ModifiedBy = "Admin";
                obj.ModifieDateTime = DateTime.Now;
                obj.IsActive = true;
                _unitOfWork.Course.Add(obj);
                _unitOfWork.Course.Save();
                TempData["success"] = "Course created successfully";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var courseFromDbFirst = _unitOfWork.Course.GetFirstOrDefault(u => u.Id == id);

            if (courseFromDbFirst == null)
            {
                return NotFound();
            }

            return View(courseFromDbFirst);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Course obj)
        {
            try
            {
                obj.ModifiedBy = "Admin";
                obj.ModifieDateTime = DateTime.Now;
                obj.IsActive = true;
                _unitOfWork.Course.Update(obj);
                _unitOfWork.Course.Save();
                TempData["success"] = "Course updated successfully";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var courseFromDbFirst = _unitOfWork.Course.GetFirstOrDefault(u => u.Id == id);

            if (courseFromDbFirst == null)
            {
                return NotFound();
            }

            return View(courseFromDbFirst);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.Course.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Course.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Course deleted successfully";
            return RedirectToAction("Index");

        }


    }
}
