﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Models.ViewModels
{
    public class SubjectViewModel
    {
        public Subject Subject { get; set; }
        public IEnumerable<SelectListItem> CourseList { get; set; }
    }
}
