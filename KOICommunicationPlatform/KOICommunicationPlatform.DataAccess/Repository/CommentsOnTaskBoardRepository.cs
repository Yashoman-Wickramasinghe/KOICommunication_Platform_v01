using KOICommunicationPlatform.DataAccess.Repository.IRepository;
using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.DataAccess.Repository
{
    public class CommentsOnTaskBoardRepository : Repository<CommentsOnTaskBoard>, ICommentsOnTaskBoardRepository
    {
        private ApplicationDbContext _db;

        public CommentsOnTaskBoardRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(CommentsOnTaskBoard obj)
        {
            _db.CommentsOnTaskBoards.Update(obj);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
