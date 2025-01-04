using KOICommunicationPlatform.DataAccess.Repository.IRepository;
using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.DataAccess.Repository
{
    public class SprintTaskAssignmentRepository : Repository<SprintTaskAssignment>, ISprintTaskAssignmentRepository
    {
        private ApplicationDbContext _db;

        public SprintTaskAssignmentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(SprintTaskAssignment obj)
        {
            _db.SprintTaskAssignments.Update(obj);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
