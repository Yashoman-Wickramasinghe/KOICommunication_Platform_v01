using KOICommunicationPlatform.Models;
using KOICommunicationPlatform.Repositories;
using KOICommunicationPlatform.Services;
using KOICommunicationPlatform.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace KOICommunicationPlatform.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ClientRegisterController : Controller
    {
        private readonly ApplicationDbContext _db;
        private IClient _client;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ClientRegisterController(ApplicationDbContext db,IClient client, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _client = client;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            List<Client> _objClientList = _db.Clients.ToList();
            return View(_objClientList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Client obj)
        {
            obj.CreatedDateTime = DateTime.Now;
            obj.ModifieDateTime = DateTime.Now;
            obj.UserRoleId = 4;
            obj.IsActive = true;
            _db.Clients.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Client? clientFromDb = _db.Clients.Find(id);

            if (clientFromDb == null)
            {
                return NotFound();
            }

            return View(clientFromDb);

        }

        [HttpPost]
        public IActionResult Edit(Client obj)
        {
            obj.CreatedDateTime = DateTime.Now;
            obj.ModifieDateTime = DateTime.Now;
            obj.UserRoleId = 4;
            obj.IsActive = true;
            _db.Clients.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        //[HttpGet]
        //public IActionResult ViewClient (int pageNumber = 1,int pageSize = 10)
        //{
        //    return View(_client.GetAll(pageNumber,pageSize));
        //}

        //View Client "Details" - Modal Pop-up
        //[HttpGet]
        //public JsonResult GetClientDetails(int id)
        //{
        //    var client = _client.GetClientById(id); 
        //    if (client == null)
        //    {
        //        return Json(new { success = false, message = "Client not found" });
        //    }
        //    return Json(new { success = true, data = client });
        //}

        //public IActionResult Edit(int id) 
        //{
        //    var viewModel = _client.GetClientById(id);
        //    return View(viewModel);
        //}

        //public IActionResult ArrangeClientMeeting()
        //{
        //    return View();
        //}
        //public IActionResult ViewClientMeeting()
        //{
        //    return View();
        //}
    }
}
