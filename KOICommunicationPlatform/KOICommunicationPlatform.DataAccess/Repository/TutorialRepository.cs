using KOICommunicationPlatform.DataAccess.Repository.IRepository;
using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.DataAccess.Repository
{
    public class TutorialRepository : Repository<Tutorial>, ITutorialRepository
    {
        private ApplicationDbContext _db;

    public TutorialRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Tutorial obj)
    {
        _db.Tutorials.Update(obj);
    }
    public void Save()
    {
        _db.SaveChanges();
    }
}
}
