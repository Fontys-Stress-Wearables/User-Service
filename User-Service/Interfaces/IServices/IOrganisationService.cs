using User_Service.Models;

namespace User_Service.Interfaces.IServices
{
    public interface IOrganisationService
    {
        public IEnumerable<Organisation> GetAll();
        public Organisation GetOrganisationByID(string id);
        public void CreateOrganisation(Organisation organisation);
        public Organisation UpdateOrganisationName(string id, string name);
        public void RemoveOrganisation(string id);
    }
}
