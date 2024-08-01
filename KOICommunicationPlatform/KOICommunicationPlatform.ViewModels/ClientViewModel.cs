using KOICommunicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.ViewModels
{
    public class ClientViewModel
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string Email { get; set; }
        public string? ContactPerson01Name { get; set; }
        public string? ContactPerson01Contact { get; set; }
        public string? ContactPerson02Name { get; set; }
        public string? ContactPerson02Contact { get; set; }
        public string? SubmissionLink { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime ModifieDateTime { get; set; } = DateTime.Now;
        public int UserRoleId { get; set; }
        public ClientViewModel() { }

        public ClientViewModel(Client model) {
            Id = model.Id;
            ClientName = model.ClientName;
            Email = model.Email;
            ContactPerson01Name = model.ContactPerson01Name;
            ContactPerson01Contact= model.ContactPerson01Contact;
            ContactPerson02Name = model.ContactPerson02Name;
            ContactPerson02Contact = model.ContactPerson02Contact;
            SubmissionLink=model.SubmissionLink;
            IsActive = model.IsActive;
            CreatedBy = model.CreatedBy;
            CreatedDateTime = model.CreatedDateTime;
            ModifiedBy = model.ModifiedBy;
            ModifieDateTime = model.ModifieDateTime;
            UserRoleId = model.UserRoleId;
        }

        public Client ConvertViewModel(ClientViewModel model)
        {
            return new Client{
                Id = model.Id,
                ClientName = model.ClientName,
                Email = model.Email,
                ContactPerson01Name = model.ContactPerson01Name,
                ContactPerson01Contact = model.ContactPerson01Contact,
                ContactPerson02Name = model.ContactPerson02Name,
                ContactPerson02Contact = model.ContactPerson02Contact,
                SubmissionLink = model.SubmissionLink,
                IsActive = model.IsActive,
                CreatedBy = model.CreatedBy,
                CreatedDateTime = model.CreatedDateTime,
                ModifiedBy = model.ModifiedBy,
                ModifieDateTime = model.ModifieDateTime,
                UserRoleId = model.UserRoleId
            };
        }
    }
}
