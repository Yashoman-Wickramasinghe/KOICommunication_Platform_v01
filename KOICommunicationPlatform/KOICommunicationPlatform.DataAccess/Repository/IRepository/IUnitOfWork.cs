﻿using KOICommunicationPlatform.DataAccess;
using KOICommunicationPlatform.DataAccess.Repository.IRepository;
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
        IProjectDeliverableRepository ProjectDeliverable { get; }
        ICourseRepository Course { get; }
        ISubjectRepository Subject { get; }
        void Save();
    }
}
