using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using User_Service.Dtos;
using User_Service.Interfaces;
using User_Service.Services;

namespace User_Service.Controllers
{
    /// <summary>
    /// Property Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IOrganisationService _organisationService;
        private readonly IMapper _mapper;
        public UserController(IMapper mapper, IOrganisationService organisationService)
        {
            _mapper = mapper;
            _organisationService = organisationService;
        }

        [HttpPost]
        public ReadOrganisationDto CreateOrganization(CreateOrganisationDto organisation)
        {
            var jeirew = _organisationService.
            var organisationData = _organisationService.CreateOrganisation(organisation.Id, organisation.Name);

            return _mapper.Map<ReadOrganisationDto>(organisationData);
        }






    }
}
