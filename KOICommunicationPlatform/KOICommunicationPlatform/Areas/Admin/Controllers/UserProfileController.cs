﻿using Microsoft.AspNetCore.Mvc;

namespace KOICommunicationPlatform.Controllers
{
    public class UserProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
