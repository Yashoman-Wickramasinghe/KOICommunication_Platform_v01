﻿using Microsoft.AspNetCore.Mvc;

namespace KOICommunicationPlatform.Areas.Client.Controllers
{
    [Area("Client")]
    public class ClientController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
