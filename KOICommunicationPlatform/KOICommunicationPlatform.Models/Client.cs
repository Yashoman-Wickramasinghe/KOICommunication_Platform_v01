using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        public Guid DocumentId { get; set; }
        [Required]
        [MaxLength(100)]
        public string ClientName { get; set; }
        [Required]
        public string Email { get; set; }
        public string? ContactPerson01Name { get; set; }
        [Phone]
        public string? ContactPerson01Contact { get; set; }
        public string? ContactPerson02Name { get; set; }
        [Phone]
        public string? ContactPerson02Contact { get; set; }
        public string? SubmissionLink {  get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifieDateTime { get; set; } = DateTime.Now;
        //[Required]
        public int UserRoleId { get; set; }
        public UserRole UserRole { get; set; }
    }
}
