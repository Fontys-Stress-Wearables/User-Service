using User_Service.Models;

namespace User_Service.Interfaces
{
    public interface IOrganisationService
    {
        public IEnumerable<Organisation> GetAll();

        public Organisation GetOrganisation(string id);

        public Organisation CreateOrganisation(string id, string name);

        public Organisation UpdateOrganisationName(string id, string name);

        public void RemoveOrganisation(string id);
    }
}
