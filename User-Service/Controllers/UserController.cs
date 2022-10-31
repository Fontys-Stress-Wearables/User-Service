using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using User_Service.Dtos;
using User_Service.Interfaces;
using User_Service.Models;
using User_Service.Services;

namespace User_Service.Controllers
{
    /// <summary>
    /// Property Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IOrganisationService _organisationService;
        private readonly ILogger<UserController> logger;
        public UserController(IOrganisationService organisationService, ILogger<UserController> logger)
        {
            _organisationService = organisationService;
            this.logger = logger;
        }

        // get organisation by id
        [HttpGet("{id}")]
        public ActionResult<ReadOrganisationDto> GetOrganisationByID(string id)
        {
            logger.LogInformation($"Getting an Organisation by the {id}");
            var organisation = _organisationService.GetOrganisationByID(id);

            if(organisation is null)
            {
                return NotFound();
            }

            return organisation.AsDto();
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



        // update organisation 
        [HttpPut("updateorganisation/{id}")]
        public ReadOrganisationDto UpdateOrganisation(string id, UpdateOrganisationDto updateOrganisationDto)
        {
            var organisationData = _organisationService.UpdateOrganisationName(id, updateOrganisationDto.Name);

            return organisationData.AsDto();
        }

        // delete organisation
        [HttpDelete("deleteorganisation/{id}")]
        public void RemoveOrganisation(string id)
        {
            _organisationService.RemoveOrganisation(id);
        }
    }
}
