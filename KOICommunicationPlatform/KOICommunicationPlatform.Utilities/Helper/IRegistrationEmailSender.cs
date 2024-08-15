using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Utilities.Helper
{
    public interface IRegistrationEmailSender
    {
        public void SendUserRegistrationEmail(ApplicationUser applicationUser, string userName);
    }
}
