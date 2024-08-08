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
    public class ClientMeeting
    {
        [Key]
        public int Id { get; set; }
        public DateTime Appointment {  get; set; }
        public string? Description { get; set; }
        public int SubjectId { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifieDateTime { get; set; } = DateTime.Now;
        public ApplicationUserClient ApplicationUserClient { get; set; }
        public Course Course { get; set; }
        public StudentGroupHD StudentGroupHD { get; set; }
    }
}
