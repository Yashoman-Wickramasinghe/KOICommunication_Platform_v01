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
    public class StudentGroupHD
    {
        [Key]
        public int Id { get; set; }
        public string CourseName { get; set; }
        public string Trimester { get; set; }
        public string Subject { get; set; }
        public int TutorialClassId { get; set; }
        public string TutorialSession { get; set; }
        public string GroupId { get; set; }
        public string? ClientName { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifieDateTime { get; set; } = DateTime.Now;
        public ApplicationUserClient ApplicationUserClient { get; set; }
        public ICollection<StudentGroupDetail> StudentGroupDetailList { get; set; }
    }
}
