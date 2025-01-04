using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Models
{
    public class SprintTasksPosition
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifieDateTime { get; set; } = DateTime.Now;
    }
}
