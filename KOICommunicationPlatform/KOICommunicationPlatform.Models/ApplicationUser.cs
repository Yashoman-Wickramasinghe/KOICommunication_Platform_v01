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
    public class ApplicationUser : IdentityUser
    {
        public int StudentId {  get; set; }
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(30)]
        public string? MiddleName { get; set; }
        [MaxLength(30)]
        public string? LastName { get; set; }
        public string? Tittle { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public Gender Gender { get; set; }
        public string? DOB { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; } 
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifieDateTime { get; set; } = DateTime.Now;
        [Required]
        public int UserRoleId { get; set; }
        public UserRole UserRole { get; set; }
    }
}

namespace KOICommunicationPlatform.Models
{
    public enum Gender
    {
        Male,Female,Other
    }
}