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
    public class Sprint
    {
        [Key]
        public int Id { get; set; }
        public string SprintName { get; set; }
        public int StudentGroupHDId { get; set; }
        public int StudentGroupDetailId {  get; set; }
        public int CourseId { get; set; }
        public int SubjectId { get; set; }
        public int UserRoleId { get; set; }
        public string ApplicationUserId { get; set; }
        public string Trimester { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifieDateTime { get; set; } = DateTime.Now;
        public Course Course { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public StudentGroupHD StudentGroupHD { get; set; }
        public StudentGroupDetail StudentGroupDetail { get; set; }
        public ICollection<SprintTask> SprintTasks { get; set; }
    }
}
