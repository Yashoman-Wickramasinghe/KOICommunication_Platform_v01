using KOICommunicationPlatform.DataAccess;
using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.DataAccess
{
    public interface IClientRepository : IRepository<Client>
    {
        void Update(Client obj);
        void Save();
    }
}
