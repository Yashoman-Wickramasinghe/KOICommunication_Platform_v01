﻿using KOICommunicationPlatform.DataAccess;
using KOICommunicationPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace KOICommunicationPlatform.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ClientRegisterController : Controller
    {
        private readonly IClientRepository _clientRepository;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ClientRegisterController(IClientRepository db, IWebHostEnvironment hostEnvironment)
        {
            _clientRepository = db;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            //List<Client> _objClientList = _clientRepository.GetAll().ToList();
            //return View(_objClientList);

            var _objClientList = _clientRepository.GetAll().ToList();

            var clientFiles = new Dictionary<int, List<string>>();

            foreach (var client in _objClientList)
            {
                if (!string.IsNullOrEmpty(client.SubmissionLink) && Directory.Exists(client.SubmissionLink))
                {
                    var files = Directory.GetFiles(client.SubmissionLink);
                    clientFiles[client.Id] = files.Select(Path.GetFileName).ToList();
                }
                else
                {
                    clientFiles[client.Id] = new List<string>();
                }
            }

            ViewBag.ClientFiles = clientFiles;

            return View(_objClientList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Client obj, IFormFile uploadedFile)
        {
            try
            {
                if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    obj.DocumentId = obj.DocumentId == Guid.Empty ? Guid.NewGuid() : obj.DocumentId;

                    string targetFolderPath = Path.Combine(_hostEnvironment.WebRootPath, "Documents", "clients", obj.DocumentId.ToString());

                    if (!Directory.Exists(targetFolderPath))
                    {
                        Directory.CreateDirectory(targetFolderPath);
                    }

                    string filePath = Path.Combine(targetFolderPath, Path.GetFileName(uploadedFile.FileName));

                    // Save only the folder path in SubmissionLink
                    obj.SubmissionLink = targetFolderPath;

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(stream);
                    }
                }

                obj.CreatedDateTime = DateTime.Now;
                obj.ModifieDateTime = DateTime.Now;
                obj.UserRoleId = 1;
                obj.IsActive = true;

                _clientRepository.Add(obj);
                _clientRepository.Save();
                TempData["success"] = "Client saved successfully";
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Client? clientFromDb = _clientRepository.Get(u=>u.Id==id);

            if (clientFromDb == null)
            {
                return NotFound();
            }
            // Extract file names from the folder specified in SubmissionLink
            List<string> fileNames = new List<string>();
            if (!string.IsNullOrEmpty(clientFromDb.SubmissionLink) && Directory.Exists(clientFromDb.SubmissionLink))
            {
                var files = Directory.GetFiles(clientFromDb.SubmissionLink);
                fileNames = files.Select(Path.GetFileName).ToList();
            }

            ViewBag.UploadedFileNames = fileNames;

            return View(clientFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Client updatedClient, IFormFile uploadedFile)
        {
            try
            {
                // Fetch the existing client from the database
                Client clientFromDb = _clientRepository.Get(u => u.Id == id);

                if (clientFromDb == null)
                {
                    return NotFound();
                }

                // Update client properties from the form values
                clientFromDb.ClientName = updatedClient.ClientName;
                clientFromDb.Email = updatedClient.Email;
                clientFromDb.ContactPerson01Name = updatedClient.ContactPerson01Name;
                clientFromDb.ContactPerson01Contact = updatedClient.ContactPerson01Contact;
                clientFromDb.ContactPerson02Name = updatedClient.ContactPerson02Name;
                clientFromDb.ContactPerson02Contact = updatedClient.ContactPerson02Contact;

                if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    // Ensure DocumentId is set
                    clientFromDb.DocumentId = clientFromDb.DocumentId == Guid.Empty ? Guid.NewGuid() : clientFromDb.DocumentId;

                    // Prepare folder and file paths
                    string targetFolderPath = Path.Combine(_hostEnvironment.WebRootPath, "Documents", "clients", clientFromDb.DocumentId.ToString());

                    if (!Directory.Exists(targetFolderPath))
                    {
                        Directory.CreateDirectory(targetFolderPath);
                    }

                    string filePath = Path.Combine(targetFolderPath, Path.GetFileName(uploadedFile.FileName));

                    // Update the SubmissionLink
                    clientFromDb.SubmissionLink = targetFolderPath;

                    // Save the uploaded file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(stream);
                    }
                }

                // Update other properties if needed
                clientFromDb.ModifieDateTime = DateTime.Now;
                clientFromDb.UserRoleId = 1;
                clientFromDb.IsActive = true;

                // Update the client in the database
                _clientRepository.Update(clientFromDb);
                _clientRepository.Save();
                //await _db.SaveChangesAsync();
                TempData["success"] = "Client edited successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Client? clientFromDb = _clientRepository.Get(u => u.Id == id);

            if (clientFromDb == null)
            {
                return NotFound();
            }

            return View(clientFromDb);

        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int id)
        {
            Client? obj = _clientRepository.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _clientRepository.Remove(obj);
            _clientRepository.Save();
            TempData["success"] = "Client record deleted successfully";
            return RedirectToAction("Index");
        }
        
    }
}