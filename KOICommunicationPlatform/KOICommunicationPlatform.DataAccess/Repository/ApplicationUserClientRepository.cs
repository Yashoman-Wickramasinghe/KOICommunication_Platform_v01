using KOICommunicationPlatform.DataAccess.Repository;
using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.DataAccess
{
    public class ApplicationUserClientRepository : Repository<ApplicationUserClient>,IApplicationUserClientRepository
    {
        private ApplicationDbContext _db;

        public ApplicationUserClientRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ApplicationUserClient obj)
        {
            _db.ApplicationUserClients.Update(obj);
        }

        public void Save() {
            _db.SaveChanges();
        }
    }
}
