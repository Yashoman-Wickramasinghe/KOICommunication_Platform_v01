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
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ClientRegisterController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            // Fetch the list of clients
            var _objClientList = _unitOfWork.ApplicationUserClient.GetAll().ToList();

            // Create a dictionary to store file names by client ID
            var clientFiles = new Dictionary<string, List<string>>();

            // Iterate over each client to collect file information
            foreach (var client in _objClientList)
            {
                // Check if SubmissionLink is not empty and directory exists
                if (!string.IsNullOrEmpty(client.SubmissionLink) && Directory.Exists(client.SubmissionLink))
                {
                    // Get the list of files in the directory
                    var files = Directory.GetFiles(client.SubmissionLink);

                    // Extract file names and add to dictionary
                    clientFiles[client.Id] = files.Select(Path.GetFileName).ToList();
                }
                else
                {
                    // If no files are found, initialize with an empty list
                    clientFiles[client.Id] = new List<string>();
                }
            }

            // Pass the client files information to the view via ViewBag
            ViewBag.ClientFiles = clientFiles;

            // Return the view with the list of clients
            return View(_objClientList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUserClient obj, IFormFile uploadedFile)
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
                obj.IsActive = true;

                _unitOfWork.ApplicationUserClient.Add(obj);
                _unitOfWork.ApplicationUserClient.Save();
                TempData["success"] = "Client saved successfully";
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult Edit(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            ApplicationUserClient? clientFromDb = _unitOfWork.ApplicationUserClient.GetFirstOrDefault(u=>u.Id==id);

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
        public async Task<IActionResult> Edit(string id, ApplicationUserClient updatedClient, IFormFile uploadedFile)
        {
            try
            {
                // Fetch the existing client from the database
                ApplicationUserClient clientFromDb = _unitOfWork.ApplicationUserClient.GetFirstOrDefault(u => u.Id == id);

                if (clientFromDb == null)
                {
                    return NotFound();
                }

                // Update client properties from the form values
                clientFromDb.Organization = updatedClient.Organization;
                clientFromDb.Email = updatedClient.Email;
                clientFromDb.ContactName = updatedClient.ContactName;
                clientFromDb.ContactPhone = updatedClient.ContactPhone;
                clientFromDb.ContactPerson02Name = updatedClient.ContactPerson02Name;
                clientFromDb.ContactPerson02Phone = updatedClient.ContactPerson02Phone;

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
                clientFromDb.IsActive = true;

                // Update the client in the database
                _unitOfWork.ApplicationUserClient.Update(clientFromDb);
                _unitOfWork.ApplicationUserClient.Save();
                //await _db.SaveChangesAsync();
                TempData["success"] = "Client edited successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        public IActionResult Delete(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            ApplicationUserClient? clientFromDb = _unitOfWork.ApplicationUserClient.GetFirstOrDefault(u => u.Id == id);

            if (clientFromDb == null)
            {
                return NotFound();
            }

            return View(clientFromDb);

        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(string id)
        {
            ApplicationUserClient? obj = _unitOfWork.ApplicationUserClient.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.ApplicationUserClient.Remove(obj);
            _unitOfWork.ApplicationUserClient.Save();
            TempData["success"] = "Client record deleted successfully";
            return RedirectToAction("Index");
        }

        public IActionResult DownloadFile(string clientId, string fileName)
        {
            // Retrieve the client to get the SubmissionLink
            var client = _unitOfWork.ApplicationUserClient.GetAll().FirstOrDefault(c => c.Id == clientId);

            if (client == null || string.IsNullOrEmpty(client.SubmissionLink))
            {
                return NotFound();
            }

            var filePath = Path.Combine(client.SubmissionLink, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var contentType = "application/octet-stream"; // Default MIME type for binary files
            return File(fileBytes, contentType, fileName);
        }


    }
}
