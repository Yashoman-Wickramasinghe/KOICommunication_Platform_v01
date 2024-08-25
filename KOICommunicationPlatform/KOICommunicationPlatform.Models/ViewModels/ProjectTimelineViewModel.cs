using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Models.ViewModels
{
    public class ProjectTimelineViewModel
    {
        public List<ProjectDeliverable> Deliverables { get; set; }
        public List<DocumentUpload> Documents { get; set; }
        public List<ProjectDeliverableViewModel> DeliverablesViewModel { get; set;}
    }

    public class DocumentUploadViewModel
    {
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }
        public DateTime UploadDate { get; set; }
        public string UploadedBy { get; set; } // Combine GivenName and Surname
        public string StudentEmail { get; set; }
    }
}
