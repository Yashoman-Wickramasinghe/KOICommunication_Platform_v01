using KOICommunicationPlatform.DataAccess.Repository.IRepository;
using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.DataAccess.Repository
{
    public class StudentGroupDetailRepository : Repository<StudentGroupDetail>, IStudentGroupDetailRepository
    {
        private ApplicationDbContext _db;

        public StudentGroupDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(StudentGroupDetail obj)
        {
            _db.StudentGroupDetails.Update(obj);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
