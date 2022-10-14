using User_Service.Interfaces;
using User_Service.Models;

namespace User_Service.Data
{
    public class OrganisationRepository : GenericRepository<Organisation>, IOrganisationRepository
    {
        public OrganisationRepository(DatabaseContext context) : base(context)
        {

        }
    }
}
