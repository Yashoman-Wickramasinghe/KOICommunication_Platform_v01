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
    public class ChatGroupHD
    {
        [Key]
        public int Id { get; set; }
        public string ChatGroupName { get; set; }
        public string ChatFileLocation { get; set; }
        public string ChatType { get; set; } //individual or group
        public int CourseId { get; set; }
        public int SubjectId { get; set; }
        public int UserRoleId { get; set; }
        public string ApplicationUserId { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifieDateTime { get; set; } = DateTime.Now;
        public ApplicationUser ApplicationUser { get; set; }
        public ICollection<ChatGroupDetail> ChatGroupDetailList { get; set; }
    }
}
