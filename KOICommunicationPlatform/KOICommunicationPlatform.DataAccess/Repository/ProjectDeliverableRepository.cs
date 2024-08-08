using KOICommunicationPlatform.DataAccess.Repository.IRepository;
using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.DataAccess.Repository
{
    public class ProjectDeliverableRepository : Repository<ProjectDeliverable>, IProjectDeliverableRepository
    {
        private ApplicationDbContext _db;

        public ProjectDeliverableRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ProjectDeliverable obj)
        {
            _db.ProjectDeliverables.Update(obj);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
