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
        private readonly IMapper _mapper;
        public UserController(IMapper mapper, IOrganisationService organisationService)
        {
            _mapper = mapper;
            _organisationService = organisationService;
        }

        [HttpPost]
        public Task<ActionResult<ReadOrganisationDto>>  PostOrganization(CreateOrganisationDto organisationDto)
        {
            //if(organisation.Name == "")
            //{
            //    return NotFound();
            //}

            // store the DTO values into a model class 
            var organisationModel = new Organisation
            {
                Id = organisationDto.Id,
                Name = organisationDto.Name
            };

            // add the newly created model class tot eh database through the _organisationService
            _organisationService.CreateOrganisation(organisationModel);

            //var organisationData = _organisationService.CreateOrganisation(organisationDto.Id, organisationDto.Name);

            var organisationInfo = _mapper.Map<ReadOrganisationDto>(organisationData);
            return CreatedAtAction(nameof(GetOrganisationByID), new { id = organisationModel.Id }, organisationModel); ;
        }

        public async Task<ActionResult<GetItemDTO>> PostAsync(CreateItemDTO createItemDTO)
        {
            // creating a new item entity
            // by storing the createdItemDTO as the properties created in the Item class
            var item = new Item
            {
                Name = createItemDTO.Name,
                Description = createItemDTO.Description,
                Price = createItemDTO.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };


            await _itemsRepository.CreateAsync(item);
            // this is the return result which produces the id in the response 
            // i.e https;//localhost:7113/items/985398y3843y43

            // https;//localhost:7113/items/{id of created item}
            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }


        // get organisation by id
        [HttpGet("{id}")]
        public ReadOrganisationDto GetOrganisationByID(string id)
        {
            var organisation = _organisationService.GetOrganisation(id);

            return _mapper.Map<ReadOrganisationDto>(organisation);
        }

        // update organisation 
        [HttpPut("{id}")]
        public ReadOrganisationDto UpdateOrganisation(string id, UpdateOrganisationDto updateOrganisationDto)
        {
            var organisationData = _organisationService.UpdateOrganisationName(id, updateOrganisationDto.Name);

            return _mapper.Map<ReadOrganisationDto>(organisationData);
        }

        // delete organisation
        [HttpDelete("{id}")]
        public void RemoveOrganisation(string id)
        {
            _organisationService.RemoveOrganisation(id);
        }
    }
}
