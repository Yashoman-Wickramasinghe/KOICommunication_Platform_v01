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
    public class UserRoleAction
    {
        [Key]
        public int Id { get; set; }
        public int? Define_User_Roles { get; set; }
        public int? View_User_Roles { get; set; }
        public int? Add_Client { get; set; }
        public int? Create_Student_Group { get; set; }
        public int? View_Created_Student_Group { get; set; }
        public int? Add_Project_Deliverables { get; set; }
        public int? Share_Documents { get; set; }
        public int? Add_Comments_To_Documents { get; set; }
        public int? Create_Tasks { get; set; }
        public int? View_Tasks { get; set; }
        public int? Comment_On_Tasks { get; set; }
        public int? Chats { get; set; }
        public int? Client_Meetings { get; set; }
        public int? View_Only { get; set; }
        public int? View_and_Edit { get; set; }
        public int? View_Others_Profiles { get; set; }
        public int? Edit_User_Profile { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifieDateTime { get; set; } = DateTime.Now;
        public int UserRoleId { get; set; }
        public UserRole UserRole { get; set; }
    }
}
