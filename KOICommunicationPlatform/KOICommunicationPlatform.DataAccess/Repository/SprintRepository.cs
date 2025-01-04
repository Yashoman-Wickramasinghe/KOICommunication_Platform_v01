using KOICommunicationPlatform.DataAccess.Repository.IRepository;
using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.DataAccess.Repository
{
    public class SprintRepository : Repository<Sprint>, ISprintRepository
    {
        private ApplicationDbContext _db;

        public SprintRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Sprint obj)
        {
            _db.Sprints.Update(obj);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
