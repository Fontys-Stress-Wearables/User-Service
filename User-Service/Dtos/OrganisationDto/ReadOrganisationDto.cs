using User_Service.Models;

namespace User_Service.Dtos.OrganisationDto
{
    public class ReadOrganisationDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public virtual List<PatientGroup> PatientGroups { get; set; }
    }
}
