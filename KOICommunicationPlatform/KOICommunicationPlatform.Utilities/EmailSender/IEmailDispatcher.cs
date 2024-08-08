using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Utilities.EmailSender
{
    public interface IEmailDispatcher
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
