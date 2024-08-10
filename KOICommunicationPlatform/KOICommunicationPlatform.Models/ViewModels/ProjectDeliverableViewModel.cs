using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KOICommunicationPlatform.Models.ViewModels
{
    public class ProjectDeliverableViewModel
    {
        public int Id { get; set; }
        public string DeliverableName { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDateTime { get; set; } = DateTime.Now;
        public int CourseId { get; set; }
        public int? SubjectId { get; set; }
        public string? Trimester { get; set; }
        public List<ProjectDeliverable> ProjectDeliverables { get; set; }
        public ProjectDeliverable ProjectDeliverable { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CourseList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> SubjectList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> TrimesterList { get; set; }
    }
}
