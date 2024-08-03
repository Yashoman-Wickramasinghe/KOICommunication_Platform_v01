
using KOICommunicationPlatform.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Models
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db) 
        {
            _db = db;
            Client = new ClientRepository(_db);
        }
        public IClientRepository Client { get; private set; }
               public void Save()
        {
            _db.SaveChanges();
        }
    }
}
