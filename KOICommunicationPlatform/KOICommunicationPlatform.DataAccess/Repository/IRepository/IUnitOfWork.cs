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
        IApplicationUserClientRepository ApplicationUserClient {  get; }
        IApplicationUserStudentRepository ApplicationUserStudent {  get; }
        IApplicationUserLecturerRepository ApplicationUserLecturer {  get; }
        IProjectDeliverableRepository ProjectDeliverable { get; }
        ICourseRepository Course { get; }
        ISubjectRepository Subject { get; }
        ITutorialRepository Tutorial { get; }
        void Save();
    }
}
