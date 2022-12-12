using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using User_Service.Controllers;
using User_Service.Dtos.UserDto;
using User_Service.Interfaces.IServices;
using User_Service.Models;
using Xunit;

namespace User_Service_Test.ServiceTests.ControllerTest
{
    public class UserControllerTest
    {
        private readonly Mock<IOrganisationService> _organisationServiceStub = new();
        private readonly Mock<IUserService> _userServiceStub = new();

        private readonly Mock<IHttpContextAccessor> _httpContextStub = new();
        private readonly string _organisationId = "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba";


        [Fact]
        public void GetUserByID_WithNonexistentUser_ReturnsNotFound()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["Tenant-ID"] = _organisationId;
            _userServiceStub.Setup(service => service.GetUser(It.IsAny<Guid>().ToString(), It.IsAny<Guid>().ToString())).Returns((User)null);
            _httpContextStub.Setup(http => http.HttpContext).Returns(context);

            var controller = new UserController(_userServiceStub.Object, _organisationServiceStub.Object, _httpContextStub.Object);

            // Act
            var actualResult = controller.GetUsersById(Guid.NewGuid().ToString());

            // Assert
            Assert.IsType<NotFoundResult>(actualResult.Result);
        }

        [Fact]
        public void GetAllUsers_ReturnsAllUsers()
        {
            // Arrange 
            var context = new DefaultHttpContext();
            context.Request.Headers["Tenant-ID"] = _organisationId;

            var expectedUsers = CreateRandomUsers();
            var controller = new UserController(_userServiceStub.Object, _organisationServiceStub.Object, _httpContextStub.Object);

            _httpContextStub.Setup(http => http.HttpContext).Returns(context);
            _userServiceStub.Setup(service => service.GetAll(_organisationId))
                .Returns(expectedUsers);

            // Act           
            var result = controller.GetAllUsers();

            // Assert
            var actualResult = Assert.IsType<ActionResult<IEnumerable<ReadUserDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actualResult.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void PostUser_WithUserToCreate_ReturnsCreatedString()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["Tenant-ID"] = _organisationId;

            var userController = new UserController(_userServiceStub.Object, _organisationServiceStub.Object, _httpContextStub.Object);

            _httpContextStub.Setup(http => http.HttpContext).Returns(context);

            var patient = new CreateUserDto()
            {
                FirstName = "John",
                LastName = "Doe",
                Birthdate = DateTime.Now,
                Role = "Patient"
            };

            // Act
            var result = userController.PostUser(patient);

            // Assert
            // convert result
            var actualResult = Assert.IsType<ActionResult<ReadUserDto>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actualResult.Result);
            var issue = Assert.IsType<String>(createdAtActionResult.Value);
            Assert.Equal($"{patient.Role} {patient.FirstName} {patient.LastName} Added", issue);
        }

        private IEnumerable<User> CreateRandomUsers()
        {
            List<User> users = new List<User>();

            var patient = new User()
            {
                Id = "1234567",
                FirstName = "John",
                LastName = "Doe",
                Birthdate = DateTime.Now,
                IsActive = true,
                Organisation = CreateRandomOrganisation(),
                Role = "Patient"
            };

            var caregiver = new User()
            {
                Id = "2345678",
                FirstName = "Carian",
                LastName = "Giverian",
                Birthdate = DateTime.Now,
                IsActive = true,
                Organisation = CreateRandomOrganisation(),
                Role = "CareGiver"
            };

            users.Add(patient);
            users.Add(caregiver);
            return users;
        }

        private User UpdateUserInfo(string? firstName, string? lastName, DateTime birthdate)
        {
            var existingUser = CreateRandomUser();
            return new()
            {
                Id = existingUser.Id,
                FirstName = firstName,
                LastName = lastName,
                Birthdate = birthdate,
                IsActive = existingUser.IsActive,
                Organisation = existingUser.Organisation,
                Role = existingUser.Role,
                PatientGroups = existingUser.PatientGroups
            };
        }

        private User CreateRandomUser()
        {
            return new()
            {
                Id = "123456789",
                FirstName = "John",
                LastName = "Doe",
                Birthdate = DateTime.Now,
                IsActive = true,
                Organisation = CreateRandomOrganisation(),
                Role = "Patient",
                PatientGroups = null
            };
        }

        private Organisation CreateRandomOrganisation()
        {
            return new()
            {
                Id = "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba",
                Name = "Fontys medical",
                Users = null,
                PatientGroups = null
            };
        }
    }
}
