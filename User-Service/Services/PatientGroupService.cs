using User_Service.Exceptions;
using User_Service.Interfaces;
using User_Service.Interfaces.IServices;
using User_Service.Models;

namespace User_Service.Services
{
    public class PatientGroupService : IPatientGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly INatsService _natsService;

        public PatientGroupService(IUnitOfWork _unitOfWork)
        {
            this._unitOfWork = _unitOfWork;
        }

        //public PatientGroupService(IUnitOfWork _unitOfWork, INatsService natsService)
        //{
        //    this._unitOfWork = _unitOfWork;
        //    this._natsService = natsService;
        //}

        public PatientGroup GetPatientGroupByIdandTenant(string patientGroupId, string tenantId)
        {
            var patientGroup = _unitOfWork.PatientGroups.GetByIdAndTenant(tenantId, patientGroupId);

            if (patientGroup == null)
            {
                throw new NotFoundException($"User with id '{patientGroupId}' doesn't exist.");
            }

            return patientGroup;
        }

        public PatientGroup Create(string name, string? description, string tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new BadRequestException("Name cannot be empty.");
            }

            var org = _unitOfWork.Organisations.GetById(tenantId);

            if (org == null)
            {
                throw new NotFoundException($"Organisation with id '{tenantId}' doesn't exist.");
            }

            var patientGroup = new PatientGroup
            {
                Id = Guid.NewGuid().ToString(),
                GroupName = name,
                Description = description,
                Organisation = org
            };

            _unitOfWork.PatientGroups.Add(patientGroup);

            //_natsService.Publish("patient-group-created", patientGroup.Organisation.Id, patientGroup);
            //_natsService.Publish("th-logs", "", $"Patient-Group created with ID: '{patientGroup.Id}.'");
            _unitOfWork.Complete();
            return patientGroup;
        }

        public void Delete(string id, string tenantId)
        {
            throw new NotImplementedException();
        }



        public PatientGroup Update(string patientGroupId, string? name, string? description, string tenantId)
        {
            throw new NotImplementedException();
        }
    }
}
