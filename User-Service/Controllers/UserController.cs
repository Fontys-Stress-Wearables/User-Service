using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using User_Service.Dtos.UserDto;
using User_Service.Interfaces.IServices;
using User_Service.Models;

namespace User_Service.Controllers
{
    [Authorize]
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IOrganisationService _organisationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IUserService userService, IOrganisationService organisationService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _organisationService = organisationService;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Roles = "Organization.Admin")]
        [HttpGet]
        public ActionResult<IEnumerable<ReadUserDto>> GetAllUsers()
        {
            var users = _userService.GetAll(_httpContextAccessor.HttpContext.User.GetTenantId()!)
                        .Select(item => item.AsUserDto());
            return Ok(users);
        }

        [Authorize(Roles = "Organization.Admin, Organization.Caregiver")]
        [HttpGet("{id}")]
        public ActionResult<ReadUserDto> GetUsersById(string id)
        {
            var user = _userService.GetUser(_httpContextAccessor.HttpContext.User.GetTenantId(), id);

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
                Organisation = _organisationService.GetOrganisationByID(_httpContextAccessor.HttpContext.User.GetTenantId()),
                Role = createUserDto.Role
            };
            _userService.AddUser(userModel);

            return CreatedAtAction(nameof(GetUsersById), new { id = userModel.Id }, $"{userModel.Role} {userModel.FirstName} {userModel.LastName} Added");
        }

        [Authorize(Roles = "Organization.Admin, Organization.Caregiver")]
        [HttpPut("{id}")]
        public ReadUserDto UpdateUser(string id, UpdateUserDto updateUserDto)
        {
            var userData = _userService.UpdateUser(_httpContextAccessor.HttpContext.User.GetTenantId()!, id, updateUserDto.FirstName, updateUserDto.LastName, updateUserDto.Birthdate);
            return userData.AsUserDto();
        }
    }
}
