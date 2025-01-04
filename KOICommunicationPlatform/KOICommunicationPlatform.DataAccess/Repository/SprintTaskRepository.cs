using KOICommunicationPlatform.DataAccess.Repository.IRepository;
using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.DataAccess.Repository
{
    public class SprintTaskRepository : Repository<SprintTask>, ISprintTaskRepository
    {
        private ApplicationDbContext _db;

        public SprintTaskRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(SprintTask obj)
        {
            _db.SprintTasks.Update(obj);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
