using KOICommunicationPlatform.DataAccess.Repository.IRepository;
using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.DataAccess.Repository
{
    public class DocumentUploadRepository : Repository<DocumentUpload>, IDocumentUploadRepository
    {
        private ApplicationDbContext _db;

        public DocumentUploadRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(DocumentUpload obj)
        {
            _db.DocumentUploads.Update(obj);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
