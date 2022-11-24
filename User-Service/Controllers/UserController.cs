using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IOrganisationService organisationService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserController(IUserService userService, IOrganisationService organisationService, IHttpContextAccessor httpContextAccessor)
        {
            this.userService = userService;
            this.organisationService = organisationService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Roles = "Organization.Admin")]
        [HttpGet]
        public ActionResult<IEnumerable<ReadUserDto>> GetAllUsers()
        {
            var users = userService.GetAll(httpContextAccessor.HttpContext.User.GetTenantId()!)
                        .Select(item => item.AsUserDto());
            return Ok(users);
        }

        [Authorize(Roles = "Organization.Admin, Organization.Caregiver")]
        [HttpGet("{id}")]
        public ActionResult<ReadUserDto> GetUsersById(string id)
        {
            var user = userService.GetUser(httpContextAccessor.HttpContext.User.GetTenantId(), id);

            if (user is null)
            {
                return NotFound();
            }
            return user.AsUserDto();
        }

        [Authorize(Roles = "Organization.Admin")]
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
                Organisation = organisationService.GetOrganisationByID(httpContextAccessor.HttpContext.User.GetTenantId()),
                Role = createUserDto.Role
            };
            userService.AddUser(userModel);

            return CreatedAtAction(nameof(GetUsersById), new { id = userModel.Id }, $"{userModel.Role} {userModel.FirstName} {userModel.LastName} Added");
        }

        [Authorize(Roles = "Organization.Admin, Organization.Caregiver")]
        [HttpPut("updates/{id}")]
        public ReadUserDto UpdateUser(string id, UpdateUserDto updateUserDto)
        {
            var userData = userService.UpdateUser(httpContextAccessor.HttpContext.User.GetTenantId()!, id, updateUserDto.FirstName, updateUserDto.LastName, updateUserDto.Birthdate);
            return userData.AsUserDto();
        }
    }
}
