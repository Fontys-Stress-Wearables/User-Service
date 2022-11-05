using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using User_Service.Dtos.OrganisationDto;
using User_Service.Dtos.PatientDto;
using User_Service.Interfaces.IServices;
using User_Service.Models;
using User_Service.Services;

namespace User_Service.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IOrganisationService organisationService;

        public UserController(IUserService userService, IOrganisationService organisationService)
        {
            this.userService = userService;
            this.organisationService = organisationService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ReadUserDto>> GetAllUsers()
        {
            //var users = userService.GetAll(HttpContext.User.GetTenantId()!)
            //            .Select(item => item.AsUserDto());
            //return Ok(users);

            // -----------Use this if authentication isnt fixed -------------------
            var users = userService.GetAll("1358d9d3-b805-4ec3-a0ee-cdd35864e8ba")
            .Select(item => item.AsUserDto());
            return Ok(users);
            // ---------------------------------------------------------------------
        }

        [HttpGet("{id}")]
        public ActionResult<ReadUserDto> GetUsersById(string id)
        {
            //var user = userService.GetUser(HttpContext.User.GetTenantId(), Id);

            // -----------Use this if authentication isnt fixed -------------------
            var user = userService.GetUser("1358d9d3-b805-4ec3-a0ee-cdd35864e8ba", id);
            // ---------------------------------------------------------------------

            if (user is null)
            {
                return NotFound();
            }
            return user.AsUserDto();
        }

        [HttpPost]
        public ActionResult<ReadUserDto> PostUser(CreateUserDto createUserDto)
        {
            var userModel = new User
            {
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Birthdate = createUserDto.Birthdate,
                IsActive = true,
                Id = Guid.NewGuid().ToString(),
                //Organisation = organisationService.GetOrganisationByID(HttpContext.User.GetTenantId()),
                Organisation = organisationService.GetOrganisationByID("1358d9d3-b805-4ec3-a0ee-cdd35864e8ba"),
                Role = createUserDto.Role
            };
            userService.AddUser(userModel);
            return CreatedAtAction(nameof(GetUsersById), new { id = userModel.Id }, userModel);
        }

        [HttpPut("updates/{id}")]
        public ReadUserDto UpdateUser(string id, UpdateUserDto updateUserDto)
        {
            var userData = userService.UpdateUser(HttpContext.User.GetTenantId()!,id, updateUserDto.FirstName, updateUserDto.LastName, updateUserDto.Birthdate);

            return userData.AsUserDto();
        }
    }
}
