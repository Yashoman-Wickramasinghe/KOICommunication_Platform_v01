using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.DataAccess.Repository.IRepository
{
    //ICommentsOnDocumentUploadRepository
    public interface ICommentsOnDocumentUploadRepository : IRepository<CommentsOnDocumentUpload>
    {
        void Update(CommentsOnDocumentUpload obj);
        void Save();
    }
}
