using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Models
{
    public class ApplicationUserLecturer: IdentityUser
    {
        public string? Title { get; set; }
        public string FirstName { get; set; }
        [MaxLength(30)]
        public string? MiddleName { get; set; }
        [MaxLength(30)]
        public string? LastName { get; set; }
        public string? DOB { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifieDateTime { get; set; } = DateTime.Now;
    }
}
