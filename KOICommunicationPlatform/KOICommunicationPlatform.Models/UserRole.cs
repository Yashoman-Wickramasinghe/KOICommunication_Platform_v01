using System.ComponentModel.DataAnnotations;

namespace KOICommunicationPlatform.Models
{
    public class UserRole
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifieDateTime { get; set; } = DateTime.Now;
    }
}