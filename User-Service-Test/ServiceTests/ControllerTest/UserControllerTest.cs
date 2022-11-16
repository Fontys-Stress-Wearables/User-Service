using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        [Fact]
        // UnitOfWork_StateUnderTest_ExpectedBehaviour
        public void GetUserByID_WithInexisitingUser_ReturnsNotFound()
        {
            // Arrange
            // this is to create a fake instance of the organisation service class for testing


            // setup of GetOrganisationById method to return null
            // we want to test the controller takes care of null values

            // whenever the controller invokes GetOrganisationById method create any Guid for its arguement and return a null value
            userServiceStub.Setup(service => service.GetUser(It.IsAny<Guid>().ToString(), It.IsAny<Guid>().ToString())).Returns((User)null);

            organisationServiceStub.Setup(service => service.GetOrganisationByID(It.IsAny<Guid>().ToString())).Returns((Organisation)null);


            var controller = new UserController(userServiceStub.Object, organisationServiceStub.Object);


            // Act
            var actualResult = controller.GetUsersById(Guid.NewGuid().ToString());

            // Assert
            // actualReult.Result 
            // actual Result.Expected Result
            Assert.IsType<NotFoundResult>(actualResult.Result);
        }

        [Fact]
        // UnitOfWork_StateUnderTest_ExpectedBehaviour
        public void PostOrganisation_WithUserToCreate_ReturnsCreatedString()
        {
            // Arrange
            var expectedOrgnaisation = CreateRandomOrganisation();
            var organisationController = new OrganisationController(organisationServiceStub.Object, loggerStub.Object);
            var userController = new UserController(userServiceStub.Object, organisationServiceStub.Object);
            organisationServiceStub.Setup(service => service.GetOrganisationByID("1358d9d3-b805-4ec3-a0ee-cdd35864e8ba")).Returns(expectedOrgnaisation);

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
            var okResult = Assert.IsType<CreatedAtActionResult>(actualResult.Result);
            var issue = Assert.IsType<String>(okResult.Value);

            Assert.Equal($"{patient.Role} {patient.FirstName} {patient.LastName} Added", issue);
        }

        private Organisation CreateRandomOrganisation()
        {
            return new()
            {
                Id = "1358d9d3-b805-4ec3-a0ee-cdd35864e8ba",
                Name = "Fontys medical",
                User = null,
                PatientGroups = null
            };
        }
    }
}
