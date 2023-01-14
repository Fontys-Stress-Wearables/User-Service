using User_Service.Models;

namespace User_Service.Interfaces.IRepositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        // these methods + IGenericRepository methods
        public IEnumerable<User> GetAllByTenant(string tenantId);
        
        public IEnumerable<User> GetAllPatientsByTenant(string tenantId);

        public User GetByIdAndTenant(string tenantId, string patientId);

        public User UpdateUser(User user);

        public void UpdateCaregiverByTenant(ICollection<User> azureCaregivers, string tenantId);
    }
}
