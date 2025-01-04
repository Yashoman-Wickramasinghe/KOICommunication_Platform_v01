using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.DataAccess.Repository.IRepository
{
    public interface ISprintTaskRepository : IRepository<SprintTask>
    {
        void Update(SprintTask obj);
        void Save();
    }
}
