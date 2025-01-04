using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.DataAccess.Repository.IRepository
{
    public interface ICommentsOnTaskBoardRepository : IRepository<CommentsOnTaskBoard>
    {
        void Update(CommentsOnTaskBoard obj);
        void Save();
    }
}
