using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Models.ViewModels
{
    public class StudentGroupViewModel
    {
        public int Id { get; set; }
        public string? StudentId { get; set; }
        public string? GivenName { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifieDateTime { get; set; } = DateTime.Now;

        // List to hold the tutorials for the dropdown
        public IEnumerable<SelectListItem>? TutorialTypeList { get; set; }

        // List to hold students
        public IEnumerable<SelectListItem>? StudentList { get; set; }

    }
}
