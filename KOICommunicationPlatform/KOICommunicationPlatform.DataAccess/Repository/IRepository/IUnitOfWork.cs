using KOICommunicationPlatform.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Models
{
    public interface IUnitOfWork
    {
        IClientRepository Client {  get; }
        void Save();
    }
}
