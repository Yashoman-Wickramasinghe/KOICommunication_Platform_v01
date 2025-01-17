﻿using KOICommunicationPlatform.DataAccess.Repository.IRepository;
using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.DataAccess.Repository
{
    public class SubjectRepository : Repository<Subject>, ISubjectRepository
    {
        private ApplicationDbContext _db;

        public SubjectRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Subject obj)
        {
            _db.Subjects.Update(obj);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}