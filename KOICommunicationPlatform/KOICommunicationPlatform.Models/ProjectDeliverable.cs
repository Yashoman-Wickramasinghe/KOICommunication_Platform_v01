using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOICommunicationPlatform.Models
{
    public class ProjectDeliverable
    {
        [Key]
        public int Id { get; set; }
        public string DeliverableName { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedDateTime { get; set; } = DateTime.Now;
        public int? SubjectId { get; set; }
        public string? Trimester { get; set; }
        public int CourseId {  get; set; }
        public Course Course { get; set; }
        public Subject Subject { get; set; }
        public ICollection<DocumentUpload> documentUploads { get; set; }
    }
}
