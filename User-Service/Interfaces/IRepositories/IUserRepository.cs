using User_Service.Models;

namespace User_Service.Interfaces.IRepositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        public IEnumerable<User> GetAllByTenant(string tenantId);
        public User GetByIdAndTenant(string tenantId, string patientId);
        public User UpdateUser(User patient);
    }
}
