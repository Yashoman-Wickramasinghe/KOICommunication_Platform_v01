
using KOICommunicationPlatform.DataAccess;
using KOICommunicationPlatform.DataAccess.Repository;
using KOICommunicationPlatform.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Models
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db) 
        {
            _db = db;
            ProjectDeliverable = new ProjectDeliverableRepository(_db);
            Course = new CourseRepository(_db);
            Subject = new SubjectRepository(_db);
            Tutorial = new TutorialRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            StudentGroupHD = new StudentGroupHDRepository(_db);
            Student = new StudentRepository(_db);
            StudentGroupDetail = new StudentGroupDetailRepository(_db);
            DocumentUpload = new DocumentUploadRepository(_db);
            CommentsOnDocumentUpload = new CommentsOnDocumentUploadRepository(_db);
        }
        
        public IProjectDeliverableRepository ProjectDeliverable { get; private set; }
        public ICourseRepository Course { get; private set; }
        public ISubjectRepository Subject { get; private set; }
        public ITutorialRepository Tutorial { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IStudentGroupHDRepository StudentGroupHD {  get; private set; }
        public IStudentRepository Student { get; private set; }
        public IStudentGroupDetailRepository StudentGroupDetail { get; private set; }
        public IDocumentUploadRepository DocumentUpload {  get; private set; }
        public ICommentsOnDocumentUploadRepository CommentsOnDocumentUpload { get; private set; }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
