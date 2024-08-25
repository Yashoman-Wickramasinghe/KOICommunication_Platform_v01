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
        public string? Trimester { get; set; }
        public string? GroupGenerateId { get; set; }
        public Guid? ClientId { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifieDateTime { get; set; } = DateTime.Now;
        public Subject? Subject { get; set; }
        public Tutorial? Tutorial { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public ICollection<StudentGroupDetail> StudentGroupDetailList { get; set; }
    }
}
