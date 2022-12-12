using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User_Service.Dtos.OrganisationDto;
using User_Service.Dtos.PatientGroupDto;
using User_Service.Interfaces.IServices;
using User_Service.Models;

namespace User_Service.Controllers
{
    [Authorize]
    [Route("organisations")]
    [ApiController]
    public class OrganisationController : ControllerBase
    {
        private readonly IOrganisationService _organisationService;
        private readonly ILogger<OrganisationController> _logger;

        public OrganisationController(IOrganisationService organisationService, ILogger<OrganisationController> logger)
        {
            _organisationService = organisationService;
            _logger = logger;
        }

        [HttpGet("/organisations")]
        public ActionResult<IEnumerable<Organisation>> GetOrganizations()
        {
            var organisations = _organisationService.GetAll();

            if (organisations is not null)
            {
                return Ok(organisations.AsOrganisationsDto());
            }

            return NotFound("No organistions found");

        }

        [HttpGet("{id}")]
        public ActionResult<ReadOrganisationDto> GetOrganisationByID(string id)
        {
            _logger.LogInformation($"Getting an Organisation by the {id}");

            var organisation = _organisationService.GetOrganisationByID(id);

            if(organisation is null)
            {
                return NotFound($"An organisation with the Id:'{id}' does not exist.");
            }

            return organisation.AsOrganisationDto();
        }

        [HttpGet("patientgroups/{id}")]
        public ActionResult<IEnumerable<ReadPatientGroupDto>> GetOrganisationPatientGroupsByID(string id)
        {
            _logger.LogInformation($"Getting an Organisation by the {id}");
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
        [ProducesResponseType(typeof(ReadOrganisationDto), 200)]
        [ProducesResponseType(400)]
        public ActionResult<ReadOrganisationDto> PostOrganisation(CreateOrganisationDto createOrganisationDTO)
        {

            if (string.IsNullOrWhiteSpace(createOrganisationDTO.Name))
            {
                return BadRequest($"Organisation name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(createOrganisationDTO.Id))
            {
                return BadRequest($"Organisation Id cannot be empty.");
            }

            var organisationModel = new Organisation
            {
                Id = createOrganisationDTO.Id,
                Name = createOrganisationDTO.Name
            };

            var organisations = _organisationService.GetAll();

            if (organisations.Any(o => o.Id == organisationModel.Id))
            {
                return BadRequest($"Organisation with Id:'{organisationModel.Id}' already exists.");
            }

            _logger.LogInformation($"Organisation {organisationModel.Name} being created ...");

            _organisationService.CreateOrganisation(organisationModel);
            // this is the return result which produces the id in the response 
            // i.e https;//localhost:7113/items/985398y3843y43

            // if the item is created its Id will be gotten back
            // https;//localhost:7113/items/{id of created item}
            return CreatedAtAction(nameof(GetOrganisationByID), new { id = organisationModel.Id }, organisationModel);
        }

        [HttpPut("update/{id}")]
        public ActionResult<ReadOrganisationDto> UpdateOrganisation(string id, UpdateOrganisationDto updateOrganisationDto)
        {
            if (string.IsNullOrWhiteSpace(updateOrganisationDto.Name))
            {
                return BadRequest($"Organisation name cannot be empty.");
            }

            var organisationData = _organisationService.UpdateOrganisationName(id, updateOrganisationDto.Name);

            if (organisationData is null)
            {
                return BadRequest($"Organisation with Id:'{id}' does not exist.");
            }

            return organisationData.AsOrganisationDto();
        }

        [HttpDelete("delete/{id}")]
        public ActionResult<HttpResponse> RemoveOrganisation(string id)
        {
            var organisation = _organisationService.GetOrganisationByID(id);

            if (organisation is null)
            {
                return BadRequest($"Organisation with Id:'{id}' does not exist.");
            }

            _organisationService.RemoveOrganisation(id);

            return Ok($"Organisation with Id:'{id}' has been deleted.");
        }
    }
}
