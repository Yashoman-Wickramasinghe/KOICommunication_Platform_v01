using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Reflection;

namespace KOICommunicationPlatform.Models
{
    public class ApplicationUser: IdentityUser
    {
        public int StudentId { get; set; }
        [MaxLength(50)]
        public string GivenName { get; set; }
        [MaxLength(30)]
        public string? Surname { get; set; }
        public string UserType { get; set; }
        public string? Title { get; set; }
        public string? DOB { get; set; }
        public bool IsActive { get; set; }
        //public string CourseId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifieDateTime { get; set; } = DateTime.Now;
        public Course? Course { get; set; }
        public Guid DocumentId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Organization { get; set; }
        public string Website { get; set; }
        public string Location { get; set; }
        public string Industry { get; set; }
        public string ProjectType { get; set; }
        public string SpecLink { get; set; }
        public string GoogleDriveLink { get; set; }
        public string? ContactName { get; set; }
        [Phone]
        public string? ContactPhone { get; set; }
        public string? ContactPerson02Name { get; set; }
        [Phone]
        public string? ContactPerson02Phone { get; set; }
        public string? SubmissionLink { get; set; }
    }
}
