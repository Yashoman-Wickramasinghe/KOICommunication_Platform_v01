using KOICommunicationPlatform.DataAccess.Repository.IRepository;
using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.DataAccess.Repository
{
    public class TaskBoardRepository : Repository<TaskBoard>, ITaskBoardRepository
    {
        private ApplicationDbContext _db;

        public TaskBoardRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(TaskBoard obj)
        {
            _db.TaskBoards.Update(obj);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
