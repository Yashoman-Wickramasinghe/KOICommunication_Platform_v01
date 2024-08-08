using KOICommunicationPlatform.DataAccess.Repository.IRepository;
using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.DataAccess.Repository
{
    public class ApplicationUserLecturerRepository : Repository<ApplicationUserLecturer>,IApplicationUserLecturerRepository
    {
        private ApplicationDbContext _db;

        public ApplicationUserLecturerRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ApplicationUserLecturer obj)
        {
            _db.ApplicationUserLecturers.Update(obj);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
