using KOICommunicationPlatform.DataAccess.Repository.IRepository;
using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.DataAccess.Repository
{
    public class ApplicationUserStudentRepository: Repository<ApplicationUserStudent>,IApplicationUserStudentRepository
    {
        private ApplicationDbContext _db;

        public ApplicationUserStudentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ApplicationUserStudent obj)
        {
            _db.ApplicationUserStudents.Update(obj);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
