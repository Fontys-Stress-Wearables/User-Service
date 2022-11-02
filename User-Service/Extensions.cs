using User_Service.Dtos.OrganisationDto;
using User_Service.Dtos.PatientDto;
using User_Service.Models;

namespace User_Service
{
    public static class Extensions
    {
        public static ReadOrganisationDto AsOrganisationDto(this Organisation organisation)
        {
            return new ReadOrganisationDto
            {
                Id = organisation.Id,
                Name = organisation.Name,
            };
        }

        public static ReadUserDto AsUserDto(this User user)
        {
            return new ReadUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Birthdate = user.Birthdate,
                IsActive = user.IsActive
            };
        }


    }
}
