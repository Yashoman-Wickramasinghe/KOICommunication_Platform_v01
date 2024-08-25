﻿using Microsoft.AspNetCore.Identity;
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
    public class DocumentUpload
    {
        [Key]
        public int Id { get; set; }
        public int? ProjectDeliverableId { get; set; }
        public int? StudentGroupDetailId { get; set; }
        public int? StudentGroupHDId { get; set; }
        public string? FullName { get; set; }
        public int? SubjectId { get; set; }
        public string? Trimester {  get; set; }
        public string? Version {  get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifieDateTime { get; set; } = DateTime.Now;
        public Course Course { get; set; }
        public ICollection<CommentsOnDocumentUpload> CommentsOnDocuments { get; set; }
    }
}
