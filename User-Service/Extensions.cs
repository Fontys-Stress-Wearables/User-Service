using System.Collections.Generic;
using User_Service.Dtos.OrganisationDto;
using User_Service.Dtos.PatientGroupDto;
using User_Service.Dtos.UserDto;
using User_Service.Models;

namespace User_Service
{
    public static class Extensions
    {

        public static IEnumerable<ReadOrganisationDto> AsOrganisationsDto(this IEnumerable<Organisation> organisations)
        {
            var organisationsDto = new List<ReadOrganisationDto>();
            foreach (var organistation in organisations)
            {
                organisationsDto.Add(organistation.AsOrganisationDto());
            }
            return organisationsDto;
        }

        public static ReadOrganisationDto AsOrganisationDto(this Organisation organisation)
        {
            return new ReadOrganisationDto
            {
                Id = organisation.Id,
                Name = organisation.Name
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

        public static ReadPatientGroupDto AsPatientGroupDto(this PatientGroup patientGroup)
        {
            return new ReadPatientGroupDto
            {
                Id = patientGroup.Id,
                GroupName = patientGroup.GroupName,
                Description = patientGroup.Description
            };
        }


    }
}
