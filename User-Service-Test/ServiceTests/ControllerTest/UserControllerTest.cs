using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User_Service.Controllers;
using User_Service.Dtos.PatientDto;
using User_Service.Interfaces.IServices;
using User_Service.Models;
using Xunit;

namespace User_Service_Test.ServiceTests.ControllerTest
{
    public class UserControllerTest
    {
        private readonly Mock<IOrganisationService> organisationServiceStub = new Mock<IOrganisationService>();
        private readonly Mock<ILogger<OrganisationController>> loggerStub = new Mock<ILogger<OrganisationController>>();

        private readonly Mock<IUserService> userServiceStub = new Mock<IUserService>();

        private readonly Mock<IHttpContextAccessor> httpContextStub = new Mock<IHttpContextAccessor>();

        private string toBeOrganisationId = "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba";


        [Fact]
        // UnitOfWork_StateUnderTest_ExpectedBehaviour
        public void GetUserByID_WithInexisitingUser_ReturnsNotFound()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["Tenant-ID"] = toBeOrganisationId;
            userServiceStub.Setup(service => service.GetUser(It.IsAny<Guid>().ToString(), It.IsAny<Guid>().ToString())).Returns((User)null);
            httpContextStub.Setup(http => http.HttpContext).Returns(context);

            var controller = new UserController(userServiceStub.Object, organisationServiceStub.Object, httpContextStub.Object);

            // Act
            var actualResult = controller.GetUsersById(Guid.NewGuid().ToString());

            // Assert
            Assert.IsType<NotFoundResult>(actualResult.Result);
        }

        //public Book(IHeaderConfiguration headerConfiguration, IHttpContextAccessor httpContextAccessor)
        //{
        //    _httpContextAccessor = httpContextAccessor;
        //    _tenantID = headerConfiguration.GetTenantId(_httpContextAccessor);
        //}

        [Fact]
        // UnitOfWork_StateUnderTest_ExpectedBehaviour
        public void GetAllUsers_ReturnsAllUsers()
        {
            // Arrange 
            var context = new DefaultHttpContext();
            context.Request.Headers["Tenant-ID"] = toBeOrganisationId;

            var expectedUsers = CreateRandomUsers();
            var expectedOrganisation = CreateRandomOrganisation();
            var controller = new UserController(userServiceStub.Object, organisationServiceStub.Object, httpContextStub.Object);

            httpContextStub.Setup(http => http.HttpContext).Returns(context);
            userServiceStub.Setup(service => service.GetAll(toBeOrganisationId))
                .Returns(expectedUsers);

            // Act           
            var result = controller.GetAllUsers();

            // Assert
            var actualResult = Assert.IsType<ActionResult<IEnumerable<ReadUserDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actualResult.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        // UnitOfWork_StateUnderTest_ExpectedBehaviour
        public void PostUser_WithUserToCreate_ReturnsCreatedString()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["Tenant-ID"] = toBeOrganisationId;

            var expectedOrgnaisation = CreateRandomOrganisation();
            var userController = new UserController(userServiceStub.Object, organisationServiceStub.Object, httpContextStub.Object);

            httpContextStub.Setup(http => http.HttpContext).Returns(context);

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

        //ToDo
        //[Fact]
        //// UnitOfWork_StateUnderTest_ExpectedBehaviour
        //public void UpdateUser_WithUserToUpdate_ReturnsUpdateUser()
        //{
        //    // Arrange 
        //    // to update an organisation we need to create an organisation
        //    var context = new DefaultHttpContext();
        //    context.Request.Headers["Tenant-ID"] = toBeOrganisationId;

        //    var existingUser = CreateRandomUser();
        //    var expectedOrgnaisation = CreateRandomOrganisation();

        //    var userController = new UserController(userServiceStub.Object, organisationServiceStub.Object, httpContextStub.Object);

        //    var existingUserId = existingUser.Id;
        //    var userToUpdate = new UpdateUserDto()
        //    {
        //        FirstName = "Johnny",
        //        LastName = "Doe",
        //        Birthdate = new DateTime(2010, 1, 5, 4, 0, 15)
        //    };

        //    var updatedUser = UpdateUserInfo(userToUpdate.FirstName, userToUpdate.LastName, userToUpdate.Birthdate);

        //    httpContextStub.Setup(http => http.HttpContext).Returns(context);
        //    userServiceStub.Setup(service => service.GetUser(expectedOrgnaisation.Id, existingUserId))
        //        .Returns(existingUser);
        //    userServiceStub.Setup(service => service.UpdateUser(expectedOrgnaisation.Id, existingUserId, userToUpdate.FirstName, userToUpdate.LastName, userToUpdate.Birthdate))
        //        .Returns(updatedUser);

        //    // Act 
        //    var result = userController.UpdateUser(existingUserId, userToUpdate);

        //    // Assert
        //    var actualResult = Assert.IsType<ReadUserDto>(result);
        //    Assert.Equal(userToUpdate.LastName, actualResult.LastName);
        //    Assert.Equal(userToUpdate.FirstName, actualResult.FirstName);
        //    Assert.Equal(userToUpdate.Birthdate, actualResult.Birthdate);
        //}

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
