using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using User_Service.Controllers;
using User_Service.Dtos.UserDto;
using User_Service.Interfaces.IServices;
using User_Service.Models;
using Xunit;

namespace User_Service_Test.ControllerTest
{
    public class UserControllerTest
    {
        private readonly Mock<IOrganisationService> _organisationServiceStub = new();
        private readonly Mock<IUserService> _userServiceStub = new();
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new();

        private const string OrganisationId = "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba";
        
        
        [Fact]
        public void GetUserByID_WithoutExistingUser_ReturnsNotFound()
        {
            // Arrange 
            var user = GenerateAuthenticatedUser();

            //Mock IHttpContextAccessor
            var context = new DefaultHttpContext { User = user };
            _mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            //Mock IUserService
            _userServiceStub.Setup(service => service.GetUser(It.IsAny<Guid>().ToString(), It.IsAny<Guid>().ToString())).Returns((User)null);
            var controller = new UserController(_userServiceStub.Object, _organisationServiceStub.Object, _mockHttpContextAccessor.Object);

            // Act
            var actualResult = controller.GetUsersById(Guid.NewGuid().ToString());

            // Assert
            Assert.IsType<NotFoundResult>(actualResult.Result);
        }

        [Fact]
        public void GetAllUsers_ReturnsAllUsers()
        {
            // Arrange 
            var user = GenerateAuthenticatedUser();

            //Mock IHttpContextAccessor
            var context = new DefaultHttpContext { User = user };
            _mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            //Mock IUserSerivce
            var expectedUsers = CreateRandomUsers();
            _userServiceStub.Setup(service => service.GetAll(OrganisationId))
                .Returns(expectedUsers);

            var controller = new UserController(_userServiceStub.Object, _organisationServiceStub.Object, _mockHttpContextAccessor.Object);

            // Act           
            var result = controller.GetAllUsers();

            // Assert
            var actualResult = Assert.IsType<ActionResult<IEnumerable<ReadUserDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actualResult.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void GetUserByID_WithExistingUser_ReturnsUser()
        {
            // Arrange 
            var user = GenerateAuthenticatedUser();

            //Mock IHttpContextAccessor
            var context = new DefaultHttpContext { User = user };
            _mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            //Mock IUserService
            var existingUser = CreateRandomUser();
            var expectedOrganisation = CreateRandomOrganisation();
            _userServiceStub.Setup(service => service.GetUser(expectedOrganisation.Id, existingUser.Id))
                .Returns(existingUser);

            var controller = new UserController(_userServiceStub.Object, _organisationServiceStub.Object, _mockHttpContextAccessor.Object);

            // Act           
            var result = controller.GetUsersById(existingUser.Id);

            // Assert
            var actualResult = Assert.IsType<ActionResult<ReadUserDto>>(result);
            var patientUser = Assert.IsType<ReadUserDto>(actualResult.Value);
            Assert.Equal("John", patientUser.FirstName);
            Assert.Equal("Doe", patientUser.LastName);
        }

        [Fact]
        public void PostUser_WithUserToCreate_ReturnsCreatedString()
        {
            // Arrange 
            var user = GenerateAuthenticatedUser();

            //Mock IHttpContextAccessor
            var context = new DefaultHttpContext { User = user };
            _mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            //Mock IUserSerivce
            var expectedOrgnaisation = CreateRandomOrganisation();
            var userController = new UserController(_userServiceStub.Object, _organisationServiceStub.Object, _mockHttpContextAccessor.Object);

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
            var actualResult = Assert.IsType<ActionResult<ReadUserDto>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actualResult.Result);
            var issue = Assert.IsType<String>(createdAtActionResult.Value);
            Assert.Equal($"{patient.Role} {patient.FirstName} {patient.LastName} Added", issue);
        }

        [Fact]
        public void UpdateUser_WithUserToUpdate_ReturnsUpdateUser()
        {
            // Arrange 
            var user = GenerateAuthenticatedUser();

            //Mock IHttpContextAccessor
            var context = new DefaultHttpContext { User = user };
            _mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            //Mock IUserService
            var existingUser = CreateRandomUser();
            var expectedOrganisation = CreateRandomOrganisation();
            var existingUserId = existingUser.Id;

            var userToUpdate = new UpdateUserDto()
            {
                FirstName = "Johnny",
                LastName = "Doe",
                Birthdate = new DateTime(2010, 1, 5, 4, 0, 15)
            };

            var updatedUser = UpdateUserInfo(userToUpdate.FirstName, userToUpdate.LastName, userToUpdate.Birthdate);

            _userServiceStub.Setup(service => service.GetUser(expectedOrganisation.Id, existingUserId))
                .Returns(existingUser);
            _userServiceStub.Setup(service => service.UpdateUser(expectedOrganisation.Id, existingUserId, userToUpdate.FirstName, userToUpdate.LastName, userToUpdate.Birthdate))
                .Returns(updatedUser);

            var userController = new UserController(_userServiceStub.Object, _organisationServiceStub.Object, _mockHttpContextAccessor.Object);

            // Act 
            var result = userController.UpdateUser(existingUserId, userToUpdate);

            // Assert
            var actualResult = Assert.IsType<ReadUserDto>(result);
            Assert.Equal(userToUpdate.LastName, actualResult.LastName);
            Assert.Equal(userToUpdate.FirstName, actualResult.FirstName);
            Assert.Equal(userToUpdate.Birthdate, actualResult.Birthdate);
        }

        private ClaimsPrincipal GenerateAuthenticatedUser() 
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                                        new(ClaimTypes.Name, "gunnar@somecompany.com"),
                                        new("http://schemas.microsoft.com/identity/claims/tenantid", OrganisationId)
                                        // other required and custom claims
                                   }, "TestAuthentication"));
            return user;
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
