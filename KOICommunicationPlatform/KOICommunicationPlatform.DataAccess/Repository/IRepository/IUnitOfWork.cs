using KOICommunicationPlatform.DataAccess;
using KOICommunicationPlatform.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Models
{
    public interface IUnitOfWork
    {
        IApplicationUserRepository ApplicationUser {  get; }
        IProjectDeliverableRepository ProjectDeliverable { get; }
        ICourseRepository Course { get; }
        ISubjectRepository Subject { get; }
        ITutorialRepository Tutorial { get; }
        IStudentGroupHDRepository StudentGroupHD { get; }
        IStudentRepository Student {  get; }
        IStudentGroupDetailRepository StudentGroupDetail { get; }
        IDocumentUploadRepository DocumentUpload { get; }
        ICommentsOnDocumentUploadRepository CommentsOnDocumentUpload { get; }
        ISprintRepository Sprint {  get; }
        ITaskBoardRepository TaskBoard { get; }
        ISprintTaskRepository SprintTask { get; }
        ISprintTaskAssignmentRepository SprintTaskAssignment { get; }
        ICommentsOnTaskBoardRepository CommentsOnTaskBoard { get; }
        void Save();
    }
}
