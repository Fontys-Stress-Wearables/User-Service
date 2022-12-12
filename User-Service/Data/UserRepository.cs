using User_Service.Interfaces.IRepositories;
using User_Service.Models;

namespace User_Service.Data
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(DatabaseContext context) : base(context)
        {

        }
       
        // get all the patients under this tenant id 
        public IEnumerable<User> GetAllByTenant(string tenantId)
        {
            return Context.Set<User>().Where(x => x.Organisation.Id == tenantId).ToList();
        }


        public User GetByIdAndTenant(string tenantId, string patientId)
        {
            return Context.Set<User>().Where(x => x.Organisation.Id == tenantId).FirstOrDefault(x => x.Id == patientId);
        }

        public User UpdateUserInDB(User patient)
        {
            return Context.Set<User>().Update(patient).Entity;
        }
    }
}
