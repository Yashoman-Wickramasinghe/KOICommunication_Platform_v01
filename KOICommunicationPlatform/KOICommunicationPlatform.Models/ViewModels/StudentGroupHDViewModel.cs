using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Models.ViewModels
{
    public class StudentGroupModel
    { }
    public partial class StudentGroupHDViewModel
    {
        public StudentGroupHDViewModel()
        {
            this.StudentGroupsDTL = new List<StudentGroupDetailViewModel>();
        }
        public List<StudentGroupDetailViewModel> StudentGroupsDTL { get; set; }
        public int Id { get; set; }
        public string CourseName { get; set; }
        public string Trimester { get; set; }
        public string Subject { get; set; }
        public int TutorialClassId { get; set; }
        public string TutorialSession { get; set; }
        public string GroupId { get; set; }
        public string? ClientName { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifieDateTime { get; set; } = DateTime.Now;
        public IEnumerable<SelectListItem>? SubjectList { get; set; }
        public IEnumerable<SelectListItem>? DayList { get; set; }
        public IEnumerable<SelectListItem>? FromTimeList { get; set; }
        public IEnumerable<SelectListItem>? ToTimeList { get; set; }
        public IEnumerable<SelectListItem>? LabTypeList { get; set; }
        public IEnumerable<SelectListItem>? TutorialTypeList { get; set; }
        public IEnumerable<SelectListItem>? ClientList { get; set; }
        public SelectList? TutorialsDropdown { get; set; } // Add this if it is used
    }
    public partial class StudentGroupDetailViewModel
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public int StudentId { get; set; }
        public string Email { get; set; }
        public string GroupId { get; set; }
        public bool IsLeader { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifieDateTime { get; set; } = DateTime.Now;
    }
}
