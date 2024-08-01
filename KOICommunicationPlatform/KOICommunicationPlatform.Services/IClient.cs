using KOICommunicationPlatform.Utilities;
using KOICommunicationPlatform.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Services
{
    public interface IClient
    {
        PagedResult<ClientViewModel> GetAll(int pageNumber, int pageSize);
        ClientViewModel GetClientById(int id);
        void UpdateClient(ClientViewModel client);
        void InsertClient(ClientViewModel client);
        void DeleteClient(int id);
    }
}
