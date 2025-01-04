using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Models.ViewModels
{
    public class StudentTaskTrackingViewModel
    {
        public int? SubjectId { get; set; }
        public int? TutorialId { get; set; }
        public string GroupGenerateId { get; set; }
        public int? Id { get; set; }

        public Dictionary<string, List<Task>> GroupedTasks { get; set; } = new Dictionary<string, List<Task>>();
    }
}
