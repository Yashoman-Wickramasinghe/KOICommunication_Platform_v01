using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.DataAccess.Repository.IRepository
{
    public interface ISprintTaskAssignmentRepository : IRepository<SprintTaskAssignment>
    {
        void Update(SprintTaskAssignment obj);
        void Save();
    }
}
