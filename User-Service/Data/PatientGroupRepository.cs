using User_Service.Interfaces.IRepositories;
using User_Service.Models;

namespace User_Service.Data
{
    public class PatientGroupRepository : GenericRepository<PatientGroup>, IPatientGroupRepository
    {
        public PatientGroupRepository(DatabaseContext context) : base(context)
        {

        }

        public IEnumerable<PatientGroup> GetAllFromTenant(string tenantId)
        {
            throw new NotImplementedException();
        }

        public void AddUser(PatientGroup patientGroup, User user)
        {
            
        }


        public PatientGroup? GetByIdAndTenant(string id, string tenantId)
        {
            return _context.Set<PatientGroup>().Where(x => x.Organisation.Id == tenantId).FirstOrDefault(x => x.Id == id);
        }

        public void RemoveUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}
