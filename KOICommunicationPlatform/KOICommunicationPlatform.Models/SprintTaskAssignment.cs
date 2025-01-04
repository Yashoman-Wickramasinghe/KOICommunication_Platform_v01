using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Models
{
    public class SprintTaskAssignment
    {
        [Key]
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifieDateTime { get; set; } = DateTime.Now;
        //public int SprintId { get; set; }
        // Foreign keys
        public int SprintTaskId { get; set; }
        public int StudentId { get; set; }

        // Navigation properties
        public SprintTask SprintTask { get; set; }
        public Student Student { get; set; }
    }
}
