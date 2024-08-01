using KOICommunicationPlatform.Models;
using KOICommunicationPlatform.Repositories.Interfaces;
using KOICommunicationPlatform.Utilities;
using KOICommunicationPlatform.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Services
{
    public class ClientService : IClient
    {
        private IUnitOfWork _unitOfWork;

        public ClientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public PagedResult<ClientViewModel> GetAll(int pageNumber, int pageSize) 
        {
            var ClientViewModel = new ClientViewModel();
            int totalCount;
            List<ClientViewModel> vmList = new List<ClientViewModel>();
            try
            {
                int ExcludeRecords = (pageSize * pageNumber) - pageSize;

                var modelList= _unitOfWork.GenericRepository<Client>().GetAll().Skip(ExcludeRecords).Take(pageSize).ToList();

                totalCount= _unitOfWork.GenericRepository<Client>().GetAll().ToList().Count();

                vmList = ConvertModelToViewModelList(modelList);
            }
            catch (Exception)
            {
                throw;
            }
            var result = new PagedResult<ClientViewModel>{
                Data = vmList,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            return result;
        }

        public ClientViewModel GetClientById(int id)
        {
            var model = _unitOfWork.GenericRepository<Client>().GetById(id);
            var vm= new ClientViewModel(model);
            return vm;
        }

        public void InsertClient(ClientViewModel client)
        {
            var model = new ClientViewModel().ConvertViewModel(client);
            _unitOfWork.GenericRepository<Client>().Add(model);
            _unitOfWork.Save();
        }

        public void UpdateClient(ClientViewModel client)
        {
            var model = new ClientViewModel().ConvertViewModel(client);
            var ModelById = _unitOfWork.GenericRepository<Client>().GetById(model.Id);
            ModelById.ClientName = client.ClientName;
            ModelById.Email = client.Email;
            ModelById.ContactPerson01Name = client.ContactPerson01Name;
            ModelById.ContactPerson01Contact = client.ContactPerson01Contact;
            ModelById.ContactPerson02Name = client.ContactPerson02Name;
            ModelById.ContactPerson02Contact = client.ContactPerson02Contact;
            ModelById.SubmissionLink = client.SubmissionLink;
            ModelById.IsActive = client.IsActive;
            ModelById.CreatedBy = client.CreatedBy;
            ModelById.CreatedDateTime = client.CreatedDateTime;
            ModelById.ModifiedBy = client.ModifiedBy;
            ModelById.ModifieDateTime = client.ModifieDateTime;
            ModelById.UserRoleId = client.UserRoleId;
            _unitOfWork.GenericRepository<Client>().Update(ModelById);
            _unitOfWork.Save();
        }
        public void DeleteClient(int id)
        {
            var model = _unitOfWork.GenericRepository<Client>().GetById(id);
            _unitOfWork.GenericRepository<Client>().Delete(model);
            _unitOfWork.Save();
        }

        private List<ClientViewModel> ConvertModelToViewModelList(List<Client> modelList)
        {
            return modelList.Select(x=> new ClientViewModel(x)).ToList();
        }
    }
}
