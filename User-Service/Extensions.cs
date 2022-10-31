using User_Service.Dtos;
using User_Service.Models;

namespace User_Service
{
    public static class Extensions
    {
        public static ReadOrganisationDto AsDto(this Organisation organisation)
        {
            return new ReadOrganisationDto
            {
                Id = organisation.Id,
                Name = organisation.Name,
            };
        }
    }
}
