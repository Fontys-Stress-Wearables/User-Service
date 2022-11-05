using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using User_Service.Dtos.OrganisationDto;
using User_Service.Dtos.PatientGroupDto;
using User_Service.Interfaces.IServices;
using User_Service.Models;
using User_Service.Services;

namespace User_Service.Controllers
{
    /// <summary>
    /// Property Controller
    /// </summary>
    [Route("organisations")]
    [ApiController]
    public class OrganisationController : ControllerBase
    {
        private readonly IOrganisationService _organisationService;
        private readonly ILogger<OrganisationController> logger;
        public OrganisationController(IOrganisationService organisationService, ILogger<OrganisationController> logger)
        {
            _organisationService = organisationService;
            this.logger = logger;
        }

        [HttpGet("{id}")]
        public ActionResult<ReadOrganisationDto> GetOrganisationByID(string id)
        {
            logger.LogInformation($"Getting an Organisation by the {id}");
            var organisation = _organisationService.GetOrganisationByID(id);

            if(organisation is null)
            {
                return NotFound();
            }

            return organisation.AsOrganisationDto();
        }

        [HttpGet("patientgroups/{id}")]
        public ActionResult<IEnumerable<ReadPatientGroupDto>> GetOrganisationPatientGroupsByID(string id)
        {
            logger.LogInformation($"Getting an Organisation by the {id}");
            var organisation = _organisationService.GetOrganisationByID(id);

            if (organisation is null)
            {
                return NotFound();
            }

            var patientGroups = organisation.PatientGroups
                .Select(item => item.AsPatientGroupDto());

           return Ok(patientGroups);
        }

        [HttpPost]
        public ActionResult<ReadOrganisationDto> PostOrganisation(CreateOrganisationDto createOrganisationDTO)
        {
            // creating a new item entity
            // by storing the createdItemDTO as the properties created in the Item class
            var organisationModel = new Organisation
            {
                Id = createOrganisationDTO.Id,
                Name = createOrganisationDTO.Name
            };

            logger.LogInformation($"Organisation {organisationModel.Name} being created ...");

            _organisationService.CreateOrganisation(organisationModel);
            // this is the return result which produces the id in the response 
            // i.e https;//localhost:7113/items/985398y3843y43

            // if the item is created its Id will be gotten back
            // https;//localhost:7113/items/{id of created item}
            return CreatedAtAction(nameof(GetOrganisationByID), new { id = organisationModel.Id }, organisationModel);
        }


        [HttpPut("update/{id}")]
        public ReadOrganisationDto UpdateOrganisation(string id, UpdateOrganisationDto updateOrganisationDto)
        {
            var organisationData = _organisationService.UpdateOrganisationName(id, updateOrganisationDto.Name);

            return organisationData.AsOrganisationDto();
        }


        [HttpDelete("delete/{id}")]
        public void RemoveOrganisation(string id)
        {
            _organisationService.RemoveOrganisation(id);
        }
    }
}
