using System.Runtime.CompilerServices;
using User_Service.Interfaces.IRepositories;
using User_Service.Models;
using Microsoft.EntityFrameworkCore;

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
            return _context.Set<User>().Where(x => x.Organisation.Id == tenantId).ToList();
        }


        public User GetByIdAndTenant(string tenantId, string patientId)
        {
            return _context.Set<User>().Where(x => x.Organisation.Id == tenantId).FirstOrDefault(x => x.Id == patientId);
        }

        public void UpdateCaregiverByTenant(ICollection<User> azureCaregivers, string tenantId)
        {
            // get all caregivers in this organisation database
            var allCaregiversDb = _context.Set<User>().Where(x => x.Organisation.Id == tenantId && x.Role == "Caregiver").ToList();


            // for all caregivers in our database not in the azure storage compare based on teh azure Id
            // make their active false 
            foreach (var caregiver in allCaregiversDb.Where(x => !azureCaregivers.Select(y => y.Id).Contains(x.Id)))
            {
                caregiver.IsActive = false;
                _context.Entry(caregiver).State = EntityState.Modified;
            }

            // Add caregivers that are not in the old set and update their status where needed
            foreach (var caregiverInAzure in azureCaregivers)
            {
                caregiverInAzure.IsActive = true;

                if (!allCaregiversDb.Select(x => x.Id).Contains(caregiverInAzure.Id))
                {
                    allCaregiversDb.Add(caregiverInAzure);
                    _context.Entry(caregiverInAzure).State = EntityState.Added;
                }
                else
                {
                    var currentCaregiver = allCaregiversDb.First(x => x.Id == caregiverInAzure.Id);

                    if (currentCaregiver.IsActive = caregiverInAzure.IsActive) continue;

                    currentCaregiver.IsActive = caregiverInAzure.IsActive;
                    _context.Entry(caregiverInAzure).State = EntityState.Modified;
                }
            }
        }

        public User UpdateUserInDB(User patient)
        {
            return _context.Set<User>().Update(patient).Entity;
        }
    }
}
