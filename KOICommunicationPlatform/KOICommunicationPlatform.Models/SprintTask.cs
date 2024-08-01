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
    public class SprintTask
    {
        [Key]
        public int Id { get; set; }
        public int StudentGroupHDId { get; set; }
        public int StudentGroupDetailId { get; set; }
        public int UserRoleId { get; set; }
        public string ApplicationUserId { get; set; }
        public string Description { get; set; }
        public int SprintId { get; set; }
        public string Status { get; set; } //Backlog/ToDo/InProgress/Done
        public string Priority { get; set; } //High/Medium/Low
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifieDateTime { get; set; } = DateTime.Now;
        public Sprint Sprint { get; set; }
        //public ApplicationUser ApplicationUser { get; set; }
        //public StudentGroupHD StudentGroupHD { get; set; }
       // public StudentGroupDetail StudentGroupDetail { get; set; }
    }
}
