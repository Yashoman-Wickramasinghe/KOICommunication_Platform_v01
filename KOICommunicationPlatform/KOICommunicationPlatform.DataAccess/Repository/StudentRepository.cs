using KOICommunicationPlatform.DataAccess.Repository.IRepository;
using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.DataAccess.Repository
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        private ApplicationDbContext _db;

        public StudentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Student obj)
        {
            _db.Students.Update(obj);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
