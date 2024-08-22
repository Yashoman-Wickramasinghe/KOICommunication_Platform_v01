using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        public string? StudentId {  get; set; }
        public string? GivenName { get; set; }
        public string? Surname { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifieDateTime { get; set; } = DateTime.Now;
        public Subject? Subject { get; set; }
        public Tutorial? Tutorial { get; set; }
    }
}
