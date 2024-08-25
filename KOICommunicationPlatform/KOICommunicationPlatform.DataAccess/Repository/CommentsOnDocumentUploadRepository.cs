using KOICommunicationPlatform.DataAccess.Repository.IRepository;
using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.DataAccess.Repository
{
    public class CommentsOnDocumentUploadRepository : Repository<CommentsOnDocumentUpload>, ICommentsOnDocumentUploadRepository
    {
        private ApplicationDbContext _db;

        public CommentsOnDocumentUploadRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(CommentsOnDocumentUpload obj)
        {
            _db.CommentsOnDocumentUploads.Update(obj);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
