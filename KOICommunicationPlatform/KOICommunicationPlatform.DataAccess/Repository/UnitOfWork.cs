
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
            ApplicationUserClient = new ApplicationUserClientRepository(_db);
            ApplicationUserStudent = new ApplicationUserStudentRepository(_db);
            ApplicationUserLecturer = new ApplicationUserLecturerRepository(_db);
        }
        
        public IProjectDeliverableRepository ProjectDeliverable { get; private set; }
        public ICourseRepository Course { get; private set; }
        public ISubjectRepository Subject { get; private set; }
        //public void Save();
        public IApplicationUserClientRepository ApplicationUserClient { get; private set; }
        public IApplicationUserStudentRepository ApplicationUserStudent { get; private set; }
        public IApplicationUserLecturerRepository ApplicationUserLecturer { get; private set; }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
