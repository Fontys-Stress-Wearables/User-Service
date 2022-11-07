using User_Service.Interfaces.IRepositories;
using User_Service.Models;

namespace User_Service.Data
{
    public class PatientGroupRepository : GenericRepository<PatientGroup>, IPatientGroupRepository
    {
        public PatientGroupRepository(DatabaseContext context) : base(context)
        {

        }

        // return all patient groups in this organisation
        public IEnumerable<PatientGroup> GetAllFromTenant(string tenantId)
        {
            return _context.Set<PatientGroup>().Where(x => x.Organisation.Id == tenantId).ToList();
        }

        public void AddUser(PatientGroup patientGroup, User user)
        {
            patientGroup.Users.Add(user);

            _context.PatientGroups.Update(patientGroup);
        }


        public PatientGroup? GetByIdAndTenant(string id, string tenantId)
        {
            return _context.Set<PatientGroup>().Where(x => x.Organisation.Id == tenantId).FirstOrDefault(x => x.Id == id);
        }

        public void RemoveUser(PatientGroup patientGroup, User user)
        {
            patientGroup.Users.Remove(user);
            _context.Update(patientGroup);
        }
    }
}
