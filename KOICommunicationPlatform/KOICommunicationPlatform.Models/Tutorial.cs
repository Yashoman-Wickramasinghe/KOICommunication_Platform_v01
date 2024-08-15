using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Models
{
    public class Tutorial
    {
        [Key]
        public int Id { get; set; }
        public string? Day { get; set; }
        public string? FromTime { get; set; }
        public string? ToTime { get; set; }
        public string? Trimester { get; set; }
        public string? Lab { get; set; }
        public string? TutorialNo { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifieDateTime { get; set; } = DateTime.Now;
        public Subject Subject { get; set; }
    }
}
