using User_Service.Models;

namespace User_Service.Interfaces.IServices
{
    public interface IPatientGroupService
    {
        public PatientGroup GetPatientGroupByIdandTenant(string patientGroupId, string tenantId);
        public PatientGroup Update(string patientGroupId, string? name, string? description, string tenantId);
        public PatientGroup Create(string name, string? description, string tenantId);

        //public void AddPatient(string patientGroupId, string patientId, string tenantId);

        //public void RemovePatient(string patientGroupId, string patientId, string tenantId);

        //public Task AddCaregiver(string patientGroupId, string caregiverId, string tenantId);
        //public Task RemoveCaregiver(string patientGroupId, string caregiverId, string tenantId);

        public IEnumerable<PatientGroup> GetAll(string tenantId);

        //public IEnumerable<Patient> GetPatients(string id, string tenantId);
        //public IEnumerable<PatientGroup> GetForPatient(string patientId, string tenantId);

        //public IEnumerable<Caregiver> GetCaregivers(string id, string tenantId);
        //public Task<IEnumerable<PatientGroup>> GetForCaregiver(string caregiverId, string tenantId);
        public Task AddUserToPatientGroup(string patientGroupId, string userId, string tenantId);
        public List<User> GetAllCaregiversInPatientGroup(string patientGroupId, string tenantId);
        public List<User> GetAllPatientsInPatientGroup(string patientGroupId, string tenantId);
        public IEnumerable<PatientGroup> GetForPatient(string userId, string tenantId);
        public IEnumerable<PatientGroup> GetForCareGivers(string userId, string tenantId);
        public void Delete(string id, string tenantId);
        public void RemoveUserFromPatientGroup(string patientGroupId, string userId, string tenantId);
    }
}
