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
    public class StudentGroupDetail
    {
        [Key]
        public int Id { get; set; }
        public string StudentName { get; set; }
        public int StudentId { get; set; }
        public string Email { get; set; }
        public string GroupId { get; set; }
        public bool IsLeader { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifieDateTime { get; set; } = DateTime.Now;
        public int? StudentGroupHDId { get; set; }
        public StudentGroupHD StudentGroupHD { get; set; }
        public ICollection<Subject> SubjectList { get; set; }
        public ICollection<DocumentUpload> documentUploads { get; set; }
    }
}
