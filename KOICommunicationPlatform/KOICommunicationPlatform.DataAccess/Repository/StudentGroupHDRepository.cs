using KOICommunicationPlatform.DataAccess.Repository.IRepository;
using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.DataAccess.Repository
{
    public class StudentGroupHDRepository : Repository<StudentGroupHD>, IStudentGroupHDRepository
    {
        private ApplicationDbContext _db;

        public StudentGroupHDRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(StudentGroupHD obj)
        {
            _db.StudentGroupHDs.Update(obj);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
