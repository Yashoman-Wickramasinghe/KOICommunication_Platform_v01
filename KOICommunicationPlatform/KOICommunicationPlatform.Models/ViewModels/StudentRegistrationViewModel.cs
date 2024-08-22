using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Models.ViewModels
{
    public class StudentRegistrationViewModel
    {
        public int? TutorialId { get; set; }
        public ApplicationUser Student { get; set; }
    }
}
