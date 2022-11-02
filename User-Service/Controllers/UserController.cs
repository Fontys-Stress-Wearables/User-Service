using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using User_Service.Dtos.OrganisationDto;
using User_Service.Dtos.PatientDto;
using User_Service.Interfaces;
using User_Service.Models;
using User_Service.Services;

namespace User_Service.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
       
        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ReadUserDto>> GetAllUsers()
        {
            var users = userService.GetAll(HttpContext.User.GetTenantId()!)
                        .Select(item => item.AsUserDto());
            return Ok(users);
        }

        [HttpGet("{id}")]
        public ActionResult<ReadUserDto> GetUsersById(string Id)
        {
            var user = userService.GetUser(HttpContext.User.GetTenantId(), Id);

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
                TenantId = HttpContext.User.GetTenantId()
            };
            userService.AddUser(userModel);
            return CreatedAtAction(nameof(GetUsersById), new { id = userModel.Id }, userModel);
        }

        [HttpPut("update/{id}")]
        public ReadUserDto UpdateUser(string id, UpdateUserDto updateUserDto)
        {
            var userData = userService.UpdateUser(HttpContext.User.GetTenantId()!,id, updateUserDto.FirstName, updateUserDto.LastName, updateUserDto.Birthdate);

            return userData.AsUserDto();
        }
    }
}
