﻿using User_Service.Exceptions;
using User_Service.Interfaces;
using User_Service.Interfaces.IServices;
using User_Service.Models;
using User = User_Service.Models.User;

namespace User_Service.Services
{
    public class PatientGroupService : IPatientGroupService
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        //private readonly INatsService _natsService;

        public PatientGroupService(IUnitOfWork _unitOfWork, IUserService userService)
        {
            this._unitOfWork = _unitOfWork;
            _userService = userService;
        }

        public PatientGroup GetPatientGroupByIdAndTenant(string patientGroupId, string tenantId)
        {
            var patientGroup = _unitOfWork.PatientGroups.GetByIdAndTenant(patientGroupId, tenantId);

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

        public List<User> GetAllPatientsInPatientGroup(string patientGroupId, string tenantId)
        {
            var patientGroup = GetPatientGroupByIdAndTenant(patientGroupId, tenantId);
            
            return patientGroup.Users.Where(user => user.Role == "Patient").ToList();
        }

        public List<User> GetAllCaregiversInPatientGroup(string patientGroupId, string tenantId)
        {
            var patientGroup = GetPatientGroupByIdAndTenant(patientGroupId, tenantId);

            return patientGroup.Users.Where(user => user.Role == "Caregiver").ToList();
        }

        public async Task AddUserToPatientGroup(string patientGroupId, string userId, string tenantId)
        {
            var patientGroup = GetPatientGroupByIdAndTenant(patientGroupId, tenantId);


            var patient = _userService.GetUser(tenantId, userId);
            if (patient != null)
            {
                _unitOfWork.PatientGroups.AddUser(patientGroup, patient);

                //_natsService.Publish("user-added-to-patientgroup", user.Id, patientGroup);
                _unitOfWork.Complete();
            }
            else
            {
                var caregiver = await _userService.GetCaregiver(tenantId, userId);
                if(caregiver != null)
                {
                    _unitOfWork.PatientGroups.AddUser(patientGroup, caregiver);

                    //_natsService.Publish("user-added-to-patientgroup", user.Id, patientGroup);
                    _unitOfWork.Complete();
                }
            }
        }

        public IEnumerable<PatientGroup> GetForPatient(string userId, string tenantId)
        {
            var patient = _userService.GetUser(tenantId, userId);
            if(patient.Role != "Patient")
            {
                throw new NotFoundException($"The Id inputted does not belong to a patient");
            }
            var patientGroups = patient.PatientGroups;
            return patientGroups;
        }

        public IEnumerable<PatientGroup> GetForCareGivers(string userId, string tenantId)
        {
            var carerGiver = _userService.GetCaregiver(tenantId, userId);
            if (carerGiver == null)
            {
                throw new NotFoundException($"The Id inputted does not belong to a caregiver");
            }
            var patientGroups = carerGiver.Result.PatientGroups;
            return patientGroups;
        }

        public void Delete(string id, string tenantId)
        {
            var group = _unitOfWork.PatientGroups.GetByIdAndTenant(id, tenantId);

            if(group == null)
            {
                throw new NotFoundException($"Patient group with id '{id}' doesn't exist.");
            }

            //_natsService.Publish("patient-group-removed", tenantId, group);
            //_natsService.Publish("th-logs", "", $"Patient-Group removed with ID: '{id}.'");
            _unitOfWork.PatientGroups.Remove(group);
            _unitOfWork.Complete();
        }


        public PatientGroup Update(string patientGroupId, string? name, string? description, string tenantId)
        {
            var group = _unitOfWork.PatientGroups.GetByIdAndTenant(patientGroupId, tenantId);

            if (group == null)
            {
                throw new NotFoundException($"Patient group with id '{patientGroupId}' doesn't exist.");
            }

            if (name != null) group.GroupName = name;
            if (description != null) group.Description = description;

            var updated = _unitOfWork.PatientGroups.Update(group);

            //_natsService.Publish("patient-group-updated", tenantId, updated);
            //_natsService.Publish("th-logs", "", $"Patient-Group updated with ID: '{updated.Id}.'");
            _unitOfWork.Complete();

            return updated;
        }

        public void RemoveUserFromPatientGroup(string patientGroupId, string userId, string tenantId)
        {
            var patientGroup = GetPatientGroupByIdAndTenant(patientGroupId, tenantId);

            var user = _userService.GetUser(tenantId, userId);
            
            _unitOfWork.PatientGroups.RemoveUser(patientGroup, user);
            _unitOfWork.Complete();
        }

        public IEnumerable<PatientGroup> GetAll(string tenantId)
        {
            var patientGroups = _unitOfWork.PatientGroups.GetAllFromTenant(tenantId);
            
            return patientGroups;
        }
    }
}
