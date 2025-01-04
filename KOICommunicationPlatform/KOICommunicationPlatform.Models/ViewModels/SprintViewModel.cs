using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Models.ViewModels
{
    public class SprintViewModel
    {
        public int Id { get; set; }
        public int StudentGroupHDId { get; set; }
        public IEnumerable<Sprint> ExistingSprints { get; set; }

        // Properties for Create Task Modal
        public List<Student> GroupStudents { get; set; }
        public List<SelectListItem> StatusOptions { get; set; }
        public List<SelectListItem> PriorityOptions { get; set; }
        // Properties to capture Task Details
        public string TaskDescription { get; set; }
        public string SelectedStatus { get; set; }
        public string SelectedPriority { get; set; }
        public DateTime TaskStartDate { get; set; }
        public DateTime TaskEndDate { get; set; }
        public List<int> SelectedStudentIds { get; set; }
        public Dictionary<string, List<SprintTask>> GroupedTasks { get; set; }
        public int SelectedSprintId { get; set; }
        //public List<SprintTask> SprintTasks { get; set; }
        public string TaskName { get; set; }

        // List of students assigned to the task
        //public List<StudentGroupViewModel> AssignedStudents { get; set; }
        // Dictionary to hold students assigned to each task
        public Dictionary<int, List<StudentGroupViewModel>> AssignedStudents { get; set; }

        //-edit
        public int TaskId { get; set; }
        public int SprintId { get; set; }
    }

}
