using User_Service.Models;

namespace User_Service.Interfaces.IServices
{
    public interface IPatientGroupService
    {
        public IEnumerable<PatientGroup> GetAll(string tenantId);
        public PatientGroup GetPatientGroupByIdandTenant(string patientGroupId, string tenantId);
        public PatientGroup Update(string patientGroupId, string? name, string? description, string tenantId);
        public PatientGroup Create(string name, string? description, string tenantId);
        public void Delete(string id, string tenantId);
        public Task AddUserToPatientGroup(string patientGroupId, string userId, string tenantId);
        public void RemoveUserFromPatientGroup(string patientGroupId, string userId, string tenantId);
        public List<User> GetAllCaregiversInPatientGroup(string patientGroupId, string tenantId);
        public List<User> GetAllPatientsInPatientGroup(string patientGroupId, string tenantId);
        public IEnumerable<PatientGroup> GetForPatient(string userId, string tenantId);
        public IEnumerable<PatientGroup> GetForCareGivers(string userId, string tenantId);
    }
}
