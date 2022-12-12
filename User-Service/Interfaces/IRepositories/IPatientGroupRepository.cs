using User_Service.Models;

namespace User_Service.Interfaces.IRepositories
{
    public interface IPatientGroupRepository : IGenericRepository<PatientGroup>
    {
        public IEnumerable<PatientGroup> GetAllFromTenant(string tenantId);
        public PatientGroup? GetByIdAndTenant(string id, string tenantId);
        public void AddUser(PatientGroup patientGroup, User user);
        public void RemoveUser(PatientGroup patientGroup, User user);
    }
}
